export const translateFlightStatus = (status: string | null): string => {
  if (!status) return 'Desconhecido';
  
  const statusMap: Record<string, string> = {
    'in_flight': 'Em Voo',
    'on_ground': 'No Solo',
    'airborne': 'No Ar',
    'scheduled': 'Programado',
    'active': 'Ativo',
    'landed': 'Aterrissado',
    'cancelled': 'Cancelado',
    'incident': 'Incidente',
    'diverted': 'Desviado',
  };

  const normalizedStatus = status.toLowerCase().replace(/[-\s]/g, '_');
  
  return statusMap[normalizedStatus] || status;
};

export const getCardinalDirection = (degrees: number): string => {
  const directions = ['N', 'NE', 'E', 'SE', 'S', 'SO', 'O', 'NO'];
  const index = Math.round(degrees / 45) % 8;
  return directions[index];
};

