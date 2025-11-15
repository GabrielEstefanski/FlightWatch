'use client';

import { useEffect, useState, useMemo, memo } from 'react';
import { MapContainer, TileLayer, Marker, Popup, useMapEvents } from 'react-leaflet';
import MarkerClusterGroup from 'react-leaflet-cluster';
import L from 'leaflet';
import 'leaflet/dist/leaflet.css';
import { FlightDto } from '@/types/flight';
import { debounce } from 'lodash';
import { getOptimizedConfig } from '@/config/performance';
import { translateFlightStatus } from '@/lib/flightUtils';

interface FlightMapProps {
  flights: FlightDto[];
  onBoundsChange: (bounds: {
    minLat: number;
    maxLat: number;
    minLon: number;
    maxLon: number;
  }) => void;
  onFlightClick?: (flight: FlightDto) => void;
  center?: [number, number];
  zoom?: number;
}

const MapEventsHandler = memo(({ onBoundsChange }: { onBoundsChange: FlightMapProps['onBoundsChange'] }) => {
  const CONFIG = getOptimizedConfig();
  
  const debouncedBoundsChange = useMemo(
    () => debounce((bounds: L.LatLngBounds) => {
      onBoundsChange({
        minLat: bounds.getSouth(),
        maxLat: bounds.getNorth(),
        minLon: bounds.getWest(),
        maxLon: bounds.getEast(),
      });
    }, CONFIG.DEBOUNCE_MAP_MOVEMENT, { leading: false, trailing: true }),
    [onBoundsChange]
  );

  const map = useMapEvents({
    moveend: () => {
      debouncedBoundsChange(map.getBounds());
    },
    zoomend: () => {
      debouncedBoundsChange(map.getBounds());
    },
  });

  useEffect(() => {
    return () => {
      debouncedBoundsChange.cancel();
    };
  }, [debouncedBoundsChange]);

  return null;
});

MapEventsHandler.displayName = 'MapEventsHandler';

const planeIconCache = new Map<string, L.DivIcon>();
const MAX_ICON_CACHE_SIZE = 72;

const getPlaneIcon = (rotation: number = 0): L.DivIcon => {
  const CONFIG = getOptimizedConfig();
  const roundedRotation = Math.round(rotation / CONFIG.ICON_ROTATION_PRECISION) * CONFIG.ICON_ROTATION_PRECISION;
  const cacheKey = `${roundedRotation}-${CONFIG.ICON_SIZE}`;
  
  if (planeIconCache.has(cacheKey)) {
    return planeIconCache.get(cacheKey)!;
  }

  if (planeIconCache.size >= MAX_ICON_CACHE_SIZE) {
    const firstKey = planeIconCache.keys().next().value;
    if (firstKey) {
      planeIconCache.delete(firstKey);
    }
  }

  const size = CONFIG.ICON_SIZE;
  const icon = L.divIcon({
    html: `
      <div style="transform: rotate(${roundedRotation}deg); display: flex; align-items: center; justify-content: center;">
        <svg width="${size}" height="${size}" viewBox="0 0 24 24" fill="#0066cc" stroke="#003d82" stroke-width="1.5">
          <path d="M21 16v-2l-8-5V3.5c0-.83-.67-1.5-1.5-1.5S10 2.67 10 3.5V9l-8 5v2l8-2.5V19l-2 1.5V22l3.5-1 3.5 1v-1.5L13 19v-5.5l8 2.5z"/>
        </svg>
      </div>
    `,
    className: 'plane-icon',
    iconSize: [size, size],
    iconAnchor: [size / 2, size / 2],
  });

  planeIconCache.set(cacheKey, icon);
  return icon;
};

const FlightMarker = memo(({ flight, onClick }: { flight: FlightDto; onClick?: (flight: FlightDto) => void }) => {
  const icon = useMemo(
    () => getPlaneIcon(flight.direction ?? 0),
    [flight.direction]
  );

  if (!flight.latitude || !flight.longitude) {
    return null;
  }

  const eventHandlers = useMemo(() => ({
    click: () => {
      if (onClick) {
        onClick(flight);
      }
    },
  }), [flight, onClick]);

  return (
    <Marker
      position={[flight.latitude, flight.longitude]}
      icon={icon}
      eventHandlers={eventHandlers}
    >
      <Popup>
        <div className="p-2 min-w-[200px]">
          <div className="font-bold text-lg text-aviation-blue-dark mb-2">
            ✈️ {flight.flightNumber}
          </div>
          <div className="space-y-1 text-sm">
            <div><strong>País Origem:</strong> {flight.airline}</div>
            <div><strong>Status:</strong> {translateFlightStatus(flight.flightStatus)}</div>
            {flight.categoryDescription && (
              <div><strong>Tipo:</strong> {flight.categoryDescription}</div>
            )}
            {flight.altitude && (
              <div><strong>Altitude:</strong> {Math.round(flight.altitude)}m</div>
            )}
            {flight.velocity && (
              <div><strong>Velocidade:</strong> {Math.round(flight.velocity * 3.6)} km/h</div>
            )}
            <div><strong>Lat/Lon:</strong> {flight.latitude.toFixed(4)}, {flight.longitude.toFixed(4)}</div>
            {flight.direction && (
              <div><strong>Direção:</strong> {Math.round(flight.direction)}°</div>
            )}
          </div>
        </div>
      </Popup>
    </Marker>
  );
}, (prevProps, nextProps) => {
  return (
    prevProps.flight.flightNumber === nextProps.flight.flightNumber &&
    prevProps.flight.latitude === nextProps.flight.latitude &&
    prevProps.flight.longitude === nextProps.flight.longitude &&
    prevProps.flight.direction === nextProps.flight.direction &&
    prevProps.onClick === nextProps.onClick
  );
});

FlightMarker.displayName = 'FlightMarker';

export const FlightMap = memo(({ flights, onBoundsChange, onFlightClick, center = [-15, -47], zoom = 4 }: FlightMapProps) => {
  const CONFIG = getOptimizedConfig();
  const [isMounted, setIsMounted] = useState(false);

  useEffect(() => {
    setIsMounted(true);
  }, []);

  const validFlights = useMemo(() => {
    return flights.filter(f => f.latitude != null && f.longitude != null);
  }, [flights]);

  const displayFlights = useMemo(() => {
    if (validFlights.length > CONFIG.MAX_RENDERED_FLIGHTS) {
      return validFlights.slice(0, CONFIG.MAX_RENDERED_FLIGHTS);
    }
    return validFlights;
  }, [validFlights, CONFIG.MAX_RENDERED_FLIGHTS]);

  if (!isMounted) {
    return (
      <div className="w-full h-full flex items-center justify-center bg-aviation-cloud">
        <div className="text-aviation-blue-dark text-xl">Carregando mapa...</div>
      </div>
    );
  }

  return (
    <MapContainer
      center={center}
      zoom={zoom}
      style={{ width: '100%', height: '100%' }}
      className="z-0"
      preferCanvas={CONFIG.PREFER_CANVAS}
      zoomControl={true}
    >
      <TileLayer
        attribution='&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a>'
        url="https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png"
        updateWhenIdle={CONFIG.UPDATE_TILES_WHEN_IDLE}
        keepBuffer={CONFIG.MAP_TILE_BUFFER}
      />
      
      <MapEventsHandler onBoundsChange={onBoundsChange} />

      <MarkerClusterGroup
        chunkedLoading
        maxClusterRadius={CONFIG.CLUSTER_RADIUS}
        spiderfyOnMaxZoom={true}
        showCoverageOnHover={false}
        zoomToBoundsOnClick={true}
        animate={CONFIG.ENABLE_CLUSTER_ANIMATIONS}
      >
        {displayFlights.map((flight) => (
          <FlightMarker
            key={`${flight.flightNumber}-${flight.latitude}-${flight.longitude}`}
            flight={flight}
            onClick={onFlightClick}
          />
        ))}
      </MarkerClusterGroup>
    </MapContainer>
  );
});

FlightMap.displayName = 'FlightMap';
