'use client';

import dynamic from 'next/dynamic';
import { useFlightTracking } from '@/hooks/useFlightTracking';
import { Plane, Wifi, WifiOff, Clock, Info, MapPin, ZoomIn, MousePointer, RefreshCw, Layers } from 'lucide-react';
import { DebugPanel } from '@/components/debug/DebugPanel';
import { FlightDetailsPanel } from '@/components/flight/FlightDetailsPanel';
import { useCallback, useMemo, useState } from 'react';
import { FlightDto } from '@/types/flight';

const FlightMap = dynamic(
  () => import('@/components/map/FlightMap').then((mod) => ({ default: mod.FlightMap })),
  { ssr: false, loading: () => <div className="w-full h-full bg-aviation-cloud flex items-center justify-center"><div className="text-aviation-blue-dark text-xl">Carregando mapa...</div></div> }
);

export default function Home() {
  const { flights, isConnected, flightCount, lastUpdate, subscribeToArea, currentSubscriptionId } = useFlightTracking();
  const [selectedFlight, setSelectedFlight] = useState<FlightDto | null>(null);

  const handleBoundsChange = useCallback((bounds: { minLat: number; maxLat: number; minLon: number; maxLon: number }) => {
    subscribeToArea(bounds);
  }, [subscribeToArea]);

  const handleFlightClick = useCallback((flight: FlightDto) => {
    setSelectedFlight(flight);
  }, []);

  const handleClosePanel = useCallback(() => {
    setSelectedFlight(null);
  }, []);

  const formattedTime = useMemo(() => {
    if (!lastUpdate) return '--:--:--';
    return lastUpdate.toLocaleTimeString('pt-BR');
  }, [lastUpdate]);

  return (
    <div className="relative w-screen h-screen overflow-hidden">
      <div className="absolute top-0 left-0 right-0 z-10 bg-linear-to-b from-aviation-blue-dark to-transparent">
        <div className="container mx-auto px-4 py-5">
          <div className="flex items-center justify-between">
            <div className="flex items-center gap-3">
              <div className="p-2.5 bg-linear-to-br from-blue-600 via-blue-500 to-sky-500 rounded-lg shadow-[0_0_18px_rgba(37,99,235,0.7)] backdrop-blur-sm">
                <Plane className="w-7 h-7 text-white drop-shadow-[0_0_9px_rgba(255,255,255,0.45)]" />
              </div>
              <div>
                <h1 className="text-2xl font-black bg-linear-to-r from-blue-600 via-blue-500 to-sky-500 bg-clip-text text-transparent drop-shadow-[0_0_17px_rgba(37,99,235,0.65)]">
                  FlightWatch
                </h1>
                <p className="text-sm font-semibold text-sky-500 drop-shadow-[0_2px_5px_rgba(0,0,0,0.45)]">Rastreamento em Tempo Real</p>
              </div>
            </div>

            <div className="flex items-center gap-6">
              <div className="flex items-center gap-2 bg-white/10 backdrop-blur-sm px-4 py-2 rounded-lg">
                {isConnected ? (
                  <>
                    <Wifi className="w-5 h-5 text-green-400" />
                    <span className="text-sm text-black font-medium">Conectado</span>
                  </>
                ) : (
                  <>
                    <WifiOff className="w-5 h-5 text-red-400" />
                    <span className="text-sm text-black font-medium">Desconectado</span>
                  </>
                )}
              </div>

              <div className="flex items-center gap-2 bg-white/10 backdrop-blur-sm px-4 py-2 rounded-lg">
                <Plane className="w-5 h-5 text-black" />
                <span className="text-sm font-bold text-black">{flightCount}</span>
                <span className="text-sm text-black font-medium">voos</span>
              </div>

              <div className="flex items-center gap-2 bg-white/10 backdrop-blur-sm px-4 py-2 rounded-lg">
                <Clock className="w-5 h-5 text-black" />
                <span className="text-sm text-black font-medium">{formattedTime}</span>
              </div>
            </div>
          </div>
        </div>
      </div>

      <div className="absolute bottom-6 left-6 z-10 bg-white/95 backdrop-blur-md rounded-2xl shadow-2xl border border-gray-200/50 p-6 max-w-sm">
        <div className="mb-4 pb-3 border-b border-gray-200/70">
          <h3 className="text-lg font-bold text-aviation-blue-dark">
            Como usar
          </h3>
        </div>
        
        <ul className="space-y-3">
          <li className="flex items-start gap-3 text-sm text-gray-700">
            <MapPin className="w-4 h-4 text-aviation-blue mt-0.5 shrink-0" />
            <span>Arraste o mapa para navegar</span>
          </li>
          <li className="flex items-start gap-3 text-sm text-gray-700">
            <ZoomIn className="w-4 h-4 text-aviation-blue mt-0.5 shrink-0" />
            <span>Dê zoom para ver mais detalhes</span>
          </li>
          <li className="flex items-start gap-3 text-sm text-gray-700">
            <MousePointer className="w-4 h-4 text-aviation-blue mt-0.5 shrink-0" />
            <span>Clique nos aviões para ver informações</span>
          </li>
          <li className="flex items-start gap-3 text-sm text-gray-700">
            <RefreshCw className="w-4 h-4 text-aviation-blue mt-0.5 shrink-0" />
            <span>Dados atualizam a cada 60 segundos</span>
          </li>
          <li className="flex items-start gap-3 text-sm text-gray-700">
            <Layers className="w-4 h-4 text-aviation-blue mt-0.5 shrink-0" />
            <span>Clusters agrupam aviões próximos</span>
          </li>
        </ul>
      </div>

      <div className="w-full h-full">
        <FlightMap 
          flights={flights} 
          onBoundsChange={handleBoundsChange}
          onFlightClick={handleFlightClick}
          center={[-15.7801, -47.9292]}
          zoom={5}
        />
      </div>

      <DebugPanel
        isConnected={isConnected}
        subscriptionId={currentSubscriptionId}
        flightCount={flightCount}
        flights={flights}
        lastUpdate={lastUpdate}
      />
    </div>
  );
}
