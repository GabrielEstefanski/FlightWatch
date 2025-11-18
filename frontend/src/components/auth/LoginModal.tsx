'use client';

import { useState } from 'react';
import { X, Mail, Lock, Loader2, User as UserIcon } from 'lucide-react';
import { LoginRequest, RegisterRequest, AuthResponse, ApiResponse, UserDto } from '@/types/auth';
import { apiClient } from '@/lib/api/client';
import { Button } from '@/components/ui/Button';

interface LoginModalProps {
  isOpen: boolean;
  onClose: () => void;
  onLoginSuccess: (accessToken: string, refreshToken: string, userData: UserDto) => void;
}

type Mode = 'login' | 'register';

export function LoginModal({ isOpen, onClose, onLoginSuccess }: LoginModalProps) {
  const [mode, setMode] = useState<Mode>('login');
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [confirmPassword, setConfirmPassword] = useState('');
  const [firstName, setFirstName] = useState('');
  const [lastName, setLastName] = useState('');
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  if (!isOpen) return null;

  const resetForm = () => {
    setEmail('');
    setPassword('');
    setConfirmPassword('');
    setFirstName('');
    setLastName('');
    setError(null);
  };

  const handleModeChange = (newMode: Mode) => {
    setMode(newMode);
    resetForm();
  };

  const handleLogin = async (e: React.FormEvent) => {
    e.preventDefault();
    setError(null);
    setLoading(true);

    try {
      const request: LoginRequest = { email, password };
      
      const response = await apiClient.fetch('/auth/login', {
        method: 'POST',
        body: JSON.stringify(request),
      });

      const data: ApiResponse<AuthResponse> = await response.json();

      if (!response.ok || !data.success || !data.data) {
        throw new Error(data.error?.message || 'Login failed');
      }

      const { accessToken, refreshToken, user } = data.data;
      onLoginSuccess(accessToken, refreshToken, user);
      resetForm();
      onClose();
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Login failed');
    } finally {
      setLoading(false);
    }
  };

  const handleRegister = async (e: React.FormEvent) => {
    e.preventDefault();
    setError(null);

    if (password !== confirmPassword) {
      setError('Passwords do not match');
      return;
    }

    setLoading(true);

    try {
      const request: RegisterRequest = {
        email,
        password,
        confirmPassword,
        firstName,
        lastName,
      };
      
      const response = await apiClient.fetch('/auth/register', {
        method: 'POST',
        body: JSON.stringify(request),
      });

      const data: ApiResponse<AuthResponse> = await response.json();

      if (!response.ok || !data.success || !data.data) {
        throw new Error(data.error?.message || 'Registration failed');
      }

      const { accessToken, refreshToken, user } = data.data;
      onLoginSuccess(accessToken, refreshToken, user);
      resetForm();
      onClose();
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Registration failed');
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/50 backdrop-blur-sm">
      <div className="bg-white rounded-2xl shadow-2xl w-full max-w-md mx-4 p-6 relative max-h-[90vh] flex flex-col">
        <div className="overflow-y-auto flex-1 pr-2 -mr-2">
        <div className="flex items-start justify-between mb-4">
          <div className="flex-1">
            <div className="flex gap-2 mb-4">
              <button
                onClick={() => handleModeChange('login')}
                className={`flex-1 py-2 px-4 rounded-lg font-semibold text-sm transition ${
                  mode === 'login'
                    ? 'bg-black text-white'
                    : 'bg-gray-100 text-gray-700 hover:bg-gray-200'
                }`}
              >
                Login
              </button>
              <button
                onClick={() => handleModeChange('register')}
                className={`flex-1 py-2 px-4 rounded-lg font-semibold text-sm transition ${
                  mode === 'register'
                    ? 'bg-black text-white'
                    : 'bg-gray-100 text-gray-700 hover:bg-gray-200'
                }`}
              >
                Register
              </button>
            </div>
          </div>
          <button
            onClick={onClose}
            className="p-2 text-gray-400 hover:text-gray-600 hover:bg-gray-100 rounded-lg transition ml-2"
          >
            <X className="w-5 h-5" />
          </button>
        </div>
        <div className="mb-4">
          <h2 className="text-2xl font-bold text-aviation-blue-dark">
            {mode === 'login' ? 'Login' : 'Create Account'}
          </h2>
          <p className="text-sm text-gray-600 mt-1">
            {mode === 'login' ? 'Enter your credentials' : 'Fill in your information'}
          </p>
        </div>

        <form onSubmit={mode === 'login' ? handleLogin : handleRegister} className="space-y-4">
          {mode === 'register' && (
            <>
              <div>
                <label htmlFor="firstName" className="block text-sm font-medium text-gray-700 mb-1">
                  First Name
                </label>
                <div className="relative">
                  <UserIcon className="absolute left-3 top-1/2 transform -translate-y-1/2 w-5 h-5 text-gray-400" />
                  <input
                    id="firstName"
                    type="text"
                    value={firstName}
                    onChange={(e) => setFirstName(e.target.value)}
                    required
                    className="w-full pl-10 pr-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-aviation-blue focus:border-transparent outline-none"
                    placeholder="John"
                  />
                </div>
              </div>

              <div>
                <label htmlFor="lastName" className="block text-sm font-medium text-gray-700 mb-1">
                  Last Name
                </label>
                <div className="relative">
                  <UserIcon className="absolute left-3 top-1/2 transform -translate-y-1/2 w-5 h-5 text-gray-400" />
                  <input
                    id="lastName"
                    type="text"
                    value={lastName}
                    onChange={(e) => setLastName(e.target.value)}
                    required
                    className="w-full pl-10 pr-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-aviation-blue focus:border-transparent outline-none"
                    placeholder="Doe"
                  />
                </div>
              </div>
            </>
          )}

          <div>
            <label htmlFor="email" className="block text-sm font-medium text-gray-700 mb-1">
              Email
            </label>
            <div className="relative">
              <Mail className="absolute left-3 top-1/2 transform -translate-y-1/2 w-5 h-5 text-gray-400" />
              <input
                id="email"
                type="email"
                value={email}
                onChange={(e) => setEmail(e.target.value)}
                required
                className="w-full pl-10 pr-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-aviation-blue focus:border-transparent outline-none"
                placeholder="your@email.com"
              />
            </div>
          </div>

          <div>
            <label htmlFor="password" className="block text-sm font-medium text-gray-700 mb-1">
              Password
            </label>
            <div className="relative">
              <Lock className="absolute left-3 top-1/2 transform -translate-y-1/2 w-5 h-5 text-gray-400" />
              <input
                id="password"
                type="password"
                value={password}
                onChange={(e) => setPassword(e.target.value)}
                required
                className="w-full pl-10 pr-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-aviation-blue focus:border-transparent outline-none"
                placeholder="••••••••"
              />
            </div>
          </div>

          {mode === 'register' && (
            <div>
              <label htmlFor="confirmPassword" className="block text-sm font-medium text-gray-700 mb-1">
                Confirm Password
              </label>
              <div className="relative">
                <Lock className="absolute left-3 top-1/2 transform -translate-y-1/2 w-5 h-5 text-gray-400" />
                <input
                  id="confirmPassword"
                  type="password"
                  value={confirmPassword}
                  onChange={(e) => setConfirmPassword(e.target.value)}
                  required
                  className="w-full pl-10 pr-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-aviation-blue focus:border-transparent outline-none"
                  placeholder="••••••••"
                />
              </div>
            </div>
          )}

          {error && (
            <div className="bg-red-50 border border-red-200 text-red-700 px-4 py-3 rounded-lg text-sm">
              {error}
            </div>
          )}

          <Button
            type="submit"
            disabled={loading}
            size="md"
            className="w-full flex items-center justify-center gap-2 mt-4 bg-black hover:bg-gray-800 text-white! cursor-pointer"
          >
            {loading ? (
              <>
                <Loader2 className="w-5 h-5 animate-spin" />
                {mode === 'login' ? 'Signing in...' : 'Creating account...'}
              </>
            ) : (
              mode === 'login' ? 'Sign In' : 'Create Account'
            )}
          </Button>
        </form>
        </div>
      </div>
    </div>
  );
}
