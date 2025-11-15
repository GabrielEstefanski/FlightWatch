export const PERFORMANCE_CONFIG = {
  THROTTLE_FLIGHTS_UPDATE: 500,
  DEBOUNCE_MAP_MOVEMENT: 300,
  DEBOUNCE_SUBSCRIPTION: 400,
  MAX_RENDERED_FLIGHTS: 1000,
  CLUSTER_RADIUS: 80,
  ICON_ROTATION_PRECISION: 15,
  ICON_SIZE: 18,
  MAP_TILE_BUFFER: 1,
  ENABLE_CLUSTER_ANIMATIONS: false,
  PREFER_CANVAS: true,
  UPDATE_TILES_WHEN_IDLE: true,
};

export const PERFORMANCE_PROFILES = {
  LOW: {
    ...PERFORMANCE_CONFIG,
    THROTTLE_FLIGHTS_UPDATE: 2000,
    DEBOUNCE_MAP_MOVEMENT: 1000,
    MAX_RENDERED_FLIGHTS: 500,
    CLUSTER_RADIUS: 100,
    ICON_SIZE: 16,
  },

  MEDIUM: PERFORMANCE_CONFIG,

  HIGH: {
    ...PERFORMANCE_CONFIG,
    THROTTLE_FLIGHTS_UPDATE: 500,
    DEBOUNCE_MAP_MOVEMENT: 300,
    MAX_RENDERED_FLIGHTS: 2000,
    CLUSTER_RADIUS: 60,
    ICON_SIZE: 20,
    ENABLE_CLUSTER_ANIMATIONS: false,
  },
};

export function detectPerformanceProfile(): keyof typeof PERFORMANCE_PROFILES {
  if (typeof window === 'undefined') return 'MEDIUM';

  const isMobile = /Android|webOS|iPhone|iPad|iPod|BlackBerry|IEMobile|Opera Mini/i.test(
    navigator.userAgent
  );

  const hasHighPerformance = 
    navigator.hardwareConcurrency && navigator.hardwareConcurrency >= 8 &&
    'deviceMemory' in navigator && (navigator as any).deviceMemory >= 8;

  if (isMobile) return 'LOW';
  if (hasHighPerformance) return 'HIGH';
  return 'MEDIUM';
}

export function getOptimizedConfig() {
  const profile = detectPerformanceProfile();
  return PERFORMANCE_PROFILES[profile];
}
