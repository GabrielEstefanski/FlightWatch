'use client';

import { useState, useEffect, useCallback, useRef } from 'react';
import * as signalR from '@microsoft/signalr';
import { FlightDto, FlightUpdateDto } from '@/types/flight';
import { createHubConnection } from '@/lib/signalr/hub-connection';
import { throttle, debounce } from 'lodash';
import { getOptimizedConfig } from '@/config/performance';

interface BoundingBox {
  minLat: number;
  maxLat: number;
  minLon: number;
  maxLon: number;
}

export function useFlightTracking() {
  const CONFIG = getOptimizedConfig();
  
  const [flights, setFlights] = useState<FlightDto[]>([]);
  const [isConnected, setIsConnected] = useState(false);
  const [flightCount, setFlightCount] = useState(0);
  const [lastUpdate, setLastUpdate] = useState<Date | null>(null);
  const [currentSubscriptionId, setCurrentSubscriptionId] = useState<string | null>(null);
  
  const connectionRef = useRef<signalR.HubConnection | null>(null);
  const subscriptionTimeoutRef = useRef<NodeJS.Timeout | null>(null);
  const pendingFlightsRef = useRef<FlightDto[]>([]);

  const throttledSetFlights = useRef(
    throttle((newFlights: FlightDto[]) => {
      setFlights(newFlights);
    }, CONFIG.THROTTLE_FLIGHTS_UPDATE, { leading: true, trailing: true })
  ).current;

  useEffect(() => {
    const connection = createHubConnection();
    connectionRef.current = connection;

    connection.on('flightUpdate', (update: FlightUpdateDto) => {
      if (update.flights && update.flights.length > 0) {
        pendingFlightsRef.current = update.flights;
        throttledSetFlights(update.flights);
        setFlightCount(update.flightCount);
        setLastUpdate(new Date(update.updatedAt));
      }
    });

    connection.on('FlightDataUpdated', (data: any) => {
      console.log('FlightDataUpdated received:', data);
      if (data.flights && data.flights.length > 0) {
        pendingFlightsRef.current = data.flights;
        throttledSetFlights(data.flights);
        setFlightCount(data.flightCount);
        setLastUpdate(new Date(data.timestamp));
      }
    });

    connection.on('subscriptionConfirmed', (subscription: any) => {
      console.log('Subscription confirmed:', subscription);
      setCurrentSubscriptionId(subscription.id);
    });

    connection.on('SubscriptionCreated', (data: any) => {
      console.log('SubscriptionCreated received:', data);
      setCurrentSubscriptionId(data.subscriptionId);
    });

    connection.on('subscriptionError', (errorMessage: string) => {
      console.error('Subscription error:', errorMessage);
    });

    connection.on('unsubscribeConfirmed', () => {
      console.log('Unsubscribe confirmed');
    });

    connection.onclose(() => {
      setIsConnected(false);
    });

    connection.onreconnecting(() => {
      setIsConnected(false);
    });

    connection.onreconnected(() => {
      setIsConnected(true);
    });

    connection.start()
      .then(() => {
        setIsConnected(true);
      })
      .catch((err) => {
        setIsConnected(false);
      });

    return () => {
      if (subscriptionTimeoutRef.current) {
        clearTimeout(subscriptionTimeoutRef.current);
      }
      throttledSetFlights.cancel();
      connection.stop();
    };
  }, [throttledSetFlights]);

  const debouncedSubscribe = useRef(
    debounce(async (
      connection: signalR.HubConnection | null,
      bounds: BoundingBox,
      areaName: string,
      currentSubId: string | null
    ) => {
      try {
        if (currentSubId) {
          await connection?.invoke('UnsubscribeFromFlight');
        }

        await connection?.invoke('SubscribeToFlight', {
          areaName,
          minLatitude: bounds.minLat,
          maxLatitude: bounds.maxLat,
          minLongitude: bounds.minLon,
          maxLongitude: bounds.maxLon,
        });
      } catch (error) {
      }
    }, CONFIG.DEBOUNCE_SUBSCRIPTION, { leading: false, trailing: true })
  ).current;

  const subscribeToArea = useCallback(async (bounds: BoundingBox, areaName?: string) => {
    if (!connectionRef.current || !isConnected) {
      return;
    }

    const generatedAreaName = areaName || 
      `Area_${bounds.minLat.toFixed(2)}_${bounds.maxLat.toFixed(2)}`;

    debouncedSubscribe(connectionRef.current, bounds, generatedAreaName, currentSubscriptionId);
  }, [isConnected, currentSubscriptionId, debouncedSubscribe]);

  const unsubscribe = useCallback(async () => {
    if (!connectionRef.current || !currentSubscriptionId) return;

    try {
      await connectionRef.current.invoke('UnsubscribeFromFlight');
      setCurrentSubscriptionId(null);
      setFlights([]);
      pendingFlightsRef.current = [];
    } catch (error) {
    }
  }, [currentSubscriptionId]);

  return {
    flights,
    isConnected,
    flightCount,
    lastUpdate,
    subscribeToArea,
    unsubscribe,
    currentSubscriptionId,
  };
}
