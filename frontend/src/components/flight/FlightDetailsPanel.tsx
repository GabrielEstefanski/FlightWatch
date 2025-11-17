'use client';

import { FlightDto } from '@/types/flight';
import { X, Plane, MapPin, Compass, Gauge, Calendar, Activity } from 'lucide-react';
import { useMemo } from 'react';
import { translateFlightStatus, getCardinalDirection } from '@/lib/flightUtils';

interface FlightDetailsPanelProps {
  flight: FlightDto | null;
  onClose: () => void;
}

export const FlightDetailsPanel = ({ flight, onClose }: FlightDetailsPanelProps) => {
  const formattedDate = useMemo(() => {
    if (!flight?.flightDate) return 'N/A';
    try {
      return new Date(flight.flightDate).toLocaleDateString('en-US', {
        day: '2-digit',
        month: '2-digit',
        year: 'numeric',
        hour: '2-digit',
        minute: '2-digit',
      });
    } catch {
      return flight.flightDate;
    }
  }, [flight?.flightDate]);

  if (!flight) return null;

  return (
    <>
      <div 
        className="fixed inset-0 bg-black/20 backdrop-blur-sm z-40 transition-opacity"
        onClick={onClose}
      />

      <div className="fixed top-0 right-0 h-full w-full md:w-[450px] bg-white/95 backdrop-blur-xl shadow-2xl z-50 transform transition-transform duration-300 ease-out">
        <div className="bg-linear-to-br from-blue-600 via-blue-500 to-sky-500 p-6">
          <div className="flex items-start justify-between mb-4">
            <div className="flex items-center gap-3">
              <div className="p-3 bg-white/20 backdrop-blur-sm rounded-xl">
                <Plane className="w-7 h-7 text-white" />
              </div>
              <div>
                <h2 className="text-2xl font-black text-white">
                  {flight.flightNumber}
                </h2>
                <p className="text-sm text-blue-100 font-medium">
                  {flight.airline}
                </p>
              </div>
            </div>
            <button
              onClick={onClose}
              className="p-2 hover:bg-white/20 rounded-lg transition-colors"
              aria-label="Close"
            >
              <X className="w-6 h-6 text-white" />
            </button>
          </div>

          <div className="flex items-center gap-2">
            <div className={`px-3 py-1.5 rounded-full text-xs font-semibold flex items-center gap-1.5 ${
              flight.isLive 
                ? 'bg-green-400/90 text-green-900' 
                : 'bg-gray-400/90 text-gray-900'
            }`}>
              <Activity className="w-3.5 h-3.5" />
              {flight.isLive ? 'Live' : 'Inactive'}
            </div>
            {flight.flightStatus && (
              <div className="px-3 py-1.5 rounded-full text-xs font-semibold bg-white/20 text-white">
                {translateFlightStatus(flight.flightStatus)}
              </div>
            )}
          </div>
        </div>

        <div className="p-6 overflow-y-auto h-[calc(100%-220px)]">
          <div className="mb-6">
            <h3 className="text-sm font-bold text-gray-500 uppercase tracking-wide mb-3">
              Route
            </h3>
            <div className="bg-linear-to-r from-blue-50 to-sky-50 rounded-xl p-4 border border-blue-100">
              <div className="flex items-center justify-between">
                <div className="flex-1">
                  <div className="text-xs text-gray-500 mb-1">Origin</div>
                  <div className="text-lg font-bold text-aviation-blue-dark">
                    {flight.origin || 'N/A'}
                  </div>
                </div>
                <div className="px-4">
                  <Plane className="w-5 h-5 text-aviation-blue rotate-90" />
                </div>
                <div className="flex-1 text-right">
                  <div className="text-xs text-gray-500 mb-1">Destination</div>
                  <div className="text-lg font-bold text-aviation-blue-dark">
                    {flight.destination || 'N/A'}
                  </div>
                </div>
              </div>
            </div>
          </div>

          <div className="mb-6">
            <h3 className="text-sm font-bold text-gray-500 uppercase tracking-wide mb-3">
              Technical Information
            </h3>
            <div className="space-y-3">
              {flight.altitude !== null && (
                <div className="flex items-center gap-3 p-3 bg-gray-50 rounded-lg border border-gray-200">
                  <div className="p-2 bg-aviation-blue/10 rounded-lg">
                    <Gauge className="w-5 h-5 text-aviation-blue" />
                  </div>
                  <div className="flex-1">
                    <div className="text-xs text-gray-500">Altitude</div>
                    <div className="text-base font-bold text-gray-900">
                      {Math.round(flight.altitude).toLocaleString('en-US')} meters
                    </div>
                  </div>
                </div>
              )}

              {flight.direction !== null && (
                <div className="flex items-center gap-3 p-3 bg-gray-50 rounded-lg border border-gray-200">
                  <div className="p-2 bg-aviation-blue/10 rounded-lg">
                    <Compass className="w-5 h-5 text-aviation-blue" />
                  </div>
                  <div className="flex-1">
                    <div className="text-xs text-gray-500">Direction</div>
                    <div className="text-base font-bold text-gray-900">
                      {Math.round(flight.direction)}° ({getCardinalDirection(flight.direction)})
                    </div>
                  </div>
                </div>
              )}

              {flight.latitude !== null && flight.longitude !== null && (
                <div className="flex items-center gap-3 p-3 bg-gray-50 rounded-lg border border-gray-200">
                  <div className="p-2 bg-aviation-blue/10 rounded-lg">
                    <MapPin className="w-5 h-5 text-aviation-blue" />
                  </div>
                  <div className="flex-1">
                    <div className="text-xs text-gray-500">Coordinates</div>
                    <div className="text-sm font-mono font-bold text-gray-900">
                      {flight.latitude.toFixed(6)}, {flight.longitude.toFixed(6)}
                    </div>
                  </div>
                </div>
              )}

              {flight.flightDate && (
                <div className="flex items-center gap-3 p-3 bg-gray-50 rounded-lg border border-gray-200">
                  <div className="p-2 bg-aviation-blue/10 rounded-lg">
                    <Calendar className="w-5 h-5 text-aviation-blue" />
                  </div>
                  <div className="flex-1">
                    <div className="text-xs text-gray-500">Flight Date</div>
                    <div className="text-base font-bold text-gray-900">
                      {formattedDate}
                    </div>
                  </div>
                </div>
              )}
            </div>
          </div>
        </div>
      </div>
    </>
  );
};
