'use client';
import { useEffect, useState } from 'react';
import { useAuth } from '@/components/AuthProvider';
import { createApiClient } from '@/services/api';

export default function AppointmentsPage() {
  const { token } = useAuth();
  const [appointments, setAppointments] = useState<any[]>([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    if (token) {
      // Fetch all appointments (in a real app, filter by user role)
      createApiClient(token).get('/api/appointments')
        .then(res => setAppointments(res.data))
        .catch(err => console.error(err))
        .finally(() => setLoading(false));
    }
  }, [token]);

  if (loading) return <div className="p-6 text-black">Loading appointments...</div>;

  return (
    <div className="container mx-auto p-6">
      <h1 className="text-3xl font-bold text-gray-800 mb-6">Appointments</h1>
      <div className="bg-white shadow-md rounded-lg overflow-hidden">
        <table className="min-w-full divide-y divide-gray-200">
          <thead className="bg-gray-50">
            <tr>
              <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">Date</th>
              <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">Reason</th>
              <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">Status</th>
              <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">Actions</th>
            </tr>
          </thead>
          <tbody className="bg-white divide-y divide-gray-200">
            {appointments.map((app) => (
              <tr key={app.id}>
                <td className="px-6 py-4 text-gray-900">
                  {new Date(app.appointmentDateTime).toLocaleString()}
                </td>
                <td className="px-6 py-4 text-gray-600">{app.reason}</td>
                <td className="px-6 py-4">
                  <span className={`px-2 inline-flex text-xs leading-5 font-semibold rounded-full 
                    ${app.status === 'Scheduled' ? 'bg-blue-100 text-blue-800' : 
                      app.status === 'Completed' ? 'bg-green-100 text-green-800' : 'bg-gray-100 text-gray-800'}`}>
                    {app.status}
                  </span>
                </td>
                <td className="px-6 py-4 text-sm text-gray-500">
                  <button className="text-indigo-600 hover:text-indigo-900">Edit</button>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>
    </div>
  );
}