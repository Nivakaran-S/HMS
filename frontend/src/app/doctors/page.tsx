'use client';
import { useEffect, useState } from 'react';
import { useAuth } from '@/components/AuthProvider';
import { createApiClient } from '@/services/api';

export default function DoctorsPage() {
  const { token } = useAuth();
  const [doctors, setDoctors] = useState<any[]>([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    if (token) {
      createApiClient(token).get('/api/doctors')
        .then(res => setDoctors(res.data))
        .catch(err => console.error(err))
        .finally(() => setLoading(false));
    }
  }, [token]);

  if (loading) return <div className="p-6 text-black">Loading doctors...</div>;

  return (
    <div className="container mx-auto p-6">
      <h1 className="text-3xl font-bold text-gray-800 mb-6">Medical Staff</h1>
      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
        {doctors.map((doc) => (
          <div key={doc.id} className="bg-white shadow-md rounded-lg p-6 border border-gray-100">
            <h2 className="text-xl font-semibold text-blue-700">Dr. {doc.firstName} {doc.lastName}</h2>
            <p className="text-gray-600 mb-2">{doc.specialization}</p>
            <div className="text-sm text-gray-500 space-y-1">
              <p>📧 {doc.email}</p>
              <p>📞 {doc.phone}</p>
              <p>💰 Fee: ${doc.consultationFee}</p>
            </div>
            <div className="mt-4 flex justify-end">
               <span className={`px-2 py-1 rounded text-xs ${doc.isAvailable ? 'bg-green-100 text-green-800' : 'bg-red-100 text-red-800'}`}>
                 {doc.isAvailable ? 'Available' : 'Unavailable'}
               </span>
            </div>
          </div>
        ))}
      </div>
    </div>
  );
}