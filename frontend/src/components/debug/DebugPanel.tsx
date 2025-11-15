'use client';

import { useState, useEffect, memo } from 'react';
import { Bug, X, Wifi, WifiOff } from 'lucide-react';

interface DebugPanelProps {
  isConnected: boolean;
  subscriptionId: string | null;
  flightCount: number;
  flights: any[];
  lastUpdate: Date | null;
}

export const DebugPanel = memo(({ isConnected, subscriptionId, flightCount, flights, lastUpdate }: DebugPanelProps) => {
  const [isOpen, setIsOpen] = useState(false);
  const [updateCount, setUpdateCount] = useState(0);

  useEffect(() => {
    if (lastUpdate) {
      setUpdateCount(prev => prev + 1);
    }
  }, [lastUpdate]);

  if (!isOpen) {
    return (
      <button
        onClick={() => setIsOpen(true)}
        className="fixed bottom-6 right-6 z-50 bg-linear-to-br from-blue-600 via-blue-500 to-sky-500 text-white p-4 rounded-full shadow-[0_0_18px_rgba(37,99,235,0.7)] hover:shadow-[0_0_25px_rgba(37,99,235,0.9)] transition-all duration-300 backdrop-blur-sm"
        title="Abrir Debug Panel"
        aria-label="Abrir Debug Panel"
      >
        <Bug className="w-6 h-6 drop-shadow-[0_0_9px_rgba(255,255,255,0.45)]" />
      </button>
    );
  }

  const sampleFlights = flights.slice(0, 5);

  return (
    <div className="fixed bottom-6 right-6 z-50 bg-white/95 backdrop-blur-xl rounded-2xl shadow-2xl w-[420px] max-h-[calc(100vh-100px)] flex flex-col">
      <div className="bg-linear-to-br from-blue-600 via-blue-500 to-sky-500 px-6 py-4 rounded-t-2xl">
        <div className="flex items-center justify-between">
          <div className="flex items-center gap-3">
            <div className="p-2 bg-white/20 backdrop-blur-sm rounded-lg">
              <Bug className="w-6 h-6 text-white drop-shadow-[0_0_9px_rgba(255,255,255,0.45)]" />
            </div>
            <div>
              <h3 className="text-xl font-black text-white drop-shadow-lg">
                Debug Panel
              </h3>
              <p className="text-xs text-blue-100 font-medium">Monitoramento do Sistema</p>
            </div>
          </div>
          <button
            onClick={() => setIsOpen(false)}
            className="p-2 hover:bg-white/20 rounded-lg transition-colors"
            aria-label="Fechar"
          >
            <X className="w-6 h-6 text-white" />
          </button>
        </div>
      </div>

      <div className="flex-1 overflow-y-auto p-6 space-y-4">
        <div className="bg-linear-to-r from-gray-50 to-blue-50/30 rounded-xl p-4 border border-gray-200">
          <h4 className="text-xs font-bold text-gray-500 uppercase tracking-wide mb-3">
            Status de Conexão
          </h4>
          <div className="flex items-center gap-3 p-3 bg-white rounded-lg border border-gray-200">
            {isConnected ? (
              <>
                <Wifi className="w-5 h-5 text-green-500" />
                <div className="flex-1">
                  <div className="text-sm font-bold text-gray-900">Conectado</div>
                  <div className="text-xs text-gray-500">SignalR ativo</div>
                </div>
                <div className="px-2 py-1 bg-green-100 text-green-700 text-xs font-semibold rounded">
                  Online
                </div>
              </>
            ) : (
              <>
                <WifiOff className="w-5 h-5 text-red-500" />
                <div className="flex-1">
                  <div className="text-sm font-bold text-gray-900">Desconectado</div>
                  <div className="text-xs text-gray-500">SignalR inativo</div>
                </div>
                <div className="px-2 py-1 bg-red-100 text-red-700 text-xs font-semibold rounded">
                  Offline
                </div>
              </>
            )}
          </div>
        </div>

        <div className="bg-linear-to-r from-gray-50 to-sky-50/30 rounded-xl p-4 border border-gray-200">
          <h4 className="text-xs font-bold text-gray-500 uppercase tracking-wide mb-3">
            Estatísticas
          </h4>
          <div className="grid grid-cols-2 gap-3">
            <div className="p-3 bg-white rounded-lg border border-gray-200">
              <div className="text-xs text-gray-500 mb-1">Voos Totais</div>
              <div className="text-2xl font-black text-aviation-blue-dark">{flightCount}</div>
            </div>

            <div className="p-3 bg-white rounded-lg border border-gray-200">
              <div className="text-xs text-gray-500 mb-1">Renderizados</div>
              <div className="text-2xl font-black text-aviation-blue-dark">{flights.length}</div>
            </div>

            <div className="p-3 bg-white rounded-lg border border-gray-200">
              <div className="text-xs text-gray-500 mb-1">Atualizações</div>
              <div className="text-2xl font-black text-aviation-blue-dark">{updateCount}</div>
            </div>

            <div className="p-3 bg-white rounded-lg border border-gray-200">
              <div className="text-xs text-gray-500 mb-1">Última Atualização</div>
              <div className="text-sm font-bold text-aviation-blue-dark">
                {lastUpdate ? new Date(lastUpdate).toLocaleTimeString('pt-BR') : '--:--:--'}
              </div>
            </div>
          </div>
        </div>

        <div className="bg-linear-to-r from-gray-50 to-yellow-50/30 rounded-xl p-4 border border-gray-200">
          <h4 className="text-xs font-bold text-gray-500 uppercase tracking-wide mb-2">
            Subscription ID
          </h4>
          <div className="bg-white rounded-lg p-3 border border-gray-200">
            <code className="text-xs font-mono text-gray-700 break-all">
              {subscriptionId || 'null'}
            </code>
          </div>
        </div>

        <div className="bg-linear-to-r from-gray-50 to-emerald-50/30 rounded-xl p-4">
          <h4 className="text-xs font-bold text-gray-500 uppercase tracking-wide mb-3">
            Sample de Voos ({Math.min(5, flights.length)})
          </h4>
          <div className="bg-gray-900 rounded-lg p-4 border border-gray-200 max-h-[300px] overflow-auto">
            <pre className="text-xs text-green-400 font-mono leading-relaxed">
              {JSON.stringify(sampleFlights, null, 2)}
            </pre>
            {flights.length > 5 && (
              <div className="text-xs text-gray-400 mt-3 pt-3 border-t border-gray-700">
                ... e mais {flights.length - 5} voos
              </div>
            )}
          </div>
        </div>
      </div>
    </div>
  );
});

DebugPanel.displayName = 'DebugPanel';
