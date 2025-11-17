'use client';

import Link from 'next/link';
import { Plane, Map, User } from 'lucide-react';

export function Header() {
  return (
    <header className="bg-aviation-blue-dark text-white shadow-lg">
      <div className="container mx-auto px-4">
        <div className="flex items-center justify-between h-16">
          <Link href="/" className="flex items-center gap-3 hover:opacity-80 transition">
            <Plane className="w-8 h-8" />
            <span className="text-2xl font-bold">FlightWatch</span>
          </Link>

          <nav className="hidden md:flex items-center gap-6">
            <Link 
              href="/map" 
              className="flex items-center gap-2 hover:text-aviation-sky transition"
            >
              <Map className="w-5 h-5" />
              <span>Map</span>
            </Link>
            <Link 
              href="/flights" 
              className="flex items-center gap-2 hover:text-aviation-sky transition"
            >
              <Plane className="w-5 h-5" />
              <span>Flights</span>
            </Link>
          </nav>

          <div className="flex items-center gap-4">
            <Link 
              href="/login" 
              className="flex items-center gap-2 hover:text-aviation-sky transition"
            >
              <User className="w-5 h-5" />
              <span>Login</span>
            </Link>
          </div>
        </div>
      </div>
    </header>
  );
}
