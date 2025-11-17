export const translateFlightStatus = (status: string | null): string => {
  if (!status) return 'Unknown';
  
  const statusMap: Record<string, string> = {
    'in_flight': 'In Flight',
    'on_ground': 'On Ground',
    'airborne': 'Airborne',
    'scheduled': 'Scheduled',
    'active': 'Active',
    'landed': 'Landed',
    'cancelled': 'Cancelled',
    'incident': 'Incident',
    'diverted': 'Diverted',
  };

  const normalizedStatus = status.toLowerCase().replace(/[-\s]/g, '_');
  
  return statusMap[normalizedStatus] || status;
};

export const getCardinalDirection = (degrees: number): string => {
  const directions = ['N', 'NE', 'E', 'SE', 'S', 'SW', 'W', 'NW'];
  const index = Math.round(degrees / 45) % 8;
  return directions[index];
};
