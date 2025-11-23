'use client';
import Link from 'next/link';
import { useAuth } from './AuthProvider';

export default function Navbar() {
  const { logout, userProfile } = useAuth();

  return (
    <nav className="bg-blue-700 p-4 text-white shadow-md">
      <div className="container mx-auto flex justify-between items-center">
        <Link href="/" className="text-xl font-bold">HMS Portal</Link>
        
        <div className="hidden md:flex space-x-6">
          <Link href="/patients" className="hover:text-blue-200">Patients</Link>
          <Link href="/doctors" className="hover:text-blue-200">Doctors</Link>
          <Link href="/appointments" className="hover:text-blue-200">Appointments</Link>
          <Link href="/billing" className="hover:text-blue-200">Billing</Link>
        </div>

        <div className="flex items-center gap-4">
          <span className="text-sm">Hello, {userProfile?.firstName || 'User'}</span>
          <button 
            onClick={logout}
            className="bg-red-500 hover:bg-red-600 text-white px-4 py-2 rounded text-sm transition"
          >
            Logout
          </button>
        </div>
      </div>
    </nav>
  );
}