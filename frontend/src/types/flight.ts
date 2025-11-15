export interface FlightDto {
  flightNumber: string;
  airline: string;
  latitude: number | null;
  longitude: number | null;
  origin: string;
  destination: string;
  direction: number | null;
  flightStatus: string | null;
  flightDate: string | null;
  altitude: number | null;
  velocity: number | null;
  isLive: boolean;
  category: number | null;
  categoryDescription: string | null;
}

export interface FlightUpdateDto {
  subscriptionId: string;
  areaName: string;
  updatedAt: string;
  flightCount: number;
  flights: FlightDto[];
}

export interface SubscriptionDto {
  id: string;
  areaName: string;
  minLatitude: number;
  maxLatitude: number;
  minLongitude: number;
  maxLongitude: number;
  createdAt: string;
  lastUpdatedAt: string;
  isActive: boolean;
  updateIntervalSeconds: number;
}


