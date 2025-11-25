'use client';

import { useEffect, useState } from 'react';
import Link from 'next/link';
import Header from '@/components/Header';
import ProtectedRoute from '@/components/ProtectedRoute';
import { useApi } from '@/hooks/useApi';

type DashboardStats = {
  patients: number;
  doctors: number;
  appointments: number;
  bills: number;
};

export default function Home() {
  const api = useApi();
  const [stats, setStats] = useState<DashboardStats>({
    patients: 0,
    doctors: 0,
    appointments: 0,
    bills: 0,
  });
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    if (!api) return;

    const fetchStats = async () => {
      try {
        const [patients, doctors, appointments, bills] = await Promise.all([
          api.get('/api/patients'),
          api.get('/api/doctors'),
          api.get('/api/appointments'),
          api.get('/api/billing'),
        ]);

        setStats({
          patients: patients.data.length,
          doctors: doctors.data.length,
          appointments: appointments.data.length,
          bills: bills.data.length,
        });
      } catch (err) {
        setError('Failed to load dashboard metrics.');
        console.error(err);
      } finally {
        setLoading(false);
      }
    };

    fetchStats();
  }, [api]);

  return (
    <ProtectedRoute>
      <Header title="Hospital Overview" description="Track your operations at a glance." />

      {loading && <div className="card">Loading metrics...</div>}
      {error && <div className="card border border-red-200 text-red-700">{error}</div>}

      {!loading && !error && (
        <>
          <div className="grid grid-cols-1 gap-6 md:grid-cols-2 lg:grid-cols-4 mb-8">
            {[
              { label: 'Patients', value: stats.patients, color: 'text-blue-600 border-blue-200' },
              { label: 'Doctors', value: stats.doctors, color: 'text-green-600 border-green-200' },
              { label: 'Appointments', value: stats.appointments, color: 'text-purple-600 border-purple-200' },
              { label: 'Bills', value: stats.bills, color: 'text-orange-600 border-orange-200' },
            ].map((card) => (
              <div key={card.label} className={`card border-l-4 ${card.color}`}>
                <p className="text-sm text-gray-500">{card.label}</p>
                <p className="text-4xl font-bold">{card.value}</p>
              </div>
            ))}
          </div>

          <div className="card">
            <h2 className="text-2xl font-bold mb-4">Quick Actions</h2>
            <div className="grid grid-cols-1 gap-4 md:grid-cols-2 lg:grid-cols-4">
              <Link href="/patients" className="btn btn-primary text-center">
                Manage Patients
              </Link>
              <Link href="/doctors" className="btn btn-primary text-center">
                Manage Doctors
              </Link>
              <Link href="/appointments" className="btn btn-primary text-center">
                Manage Appointments
              </Link>
              <Link href="/billing" className="btn btn-primary text-center">
                Review Billing
              </Link>
            </div>
          </div>
        </>
      )}
    </ProtectedRoute>
  );
}
