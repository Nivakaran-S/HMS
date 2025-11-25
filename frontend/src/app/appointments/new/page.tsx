'use client';

import { useState, useEffect } from 'react';
import { useRouter } from 'next/navigation';
import { useAuth } from '@/components/AuthProvider';
import { createApiClient } from '@/services/api';
import { Patient, Doctor } from '@/types';

export default function BookAppointmentPage() {
  const { token } = useAuth();
  const router = useRouter();
  
  // In a real app, you'd search/select patient & doctor. 
  // For MVP, we'll enter IDs manually or fetch lists.
  const [patients, setPatients] = useState<Patient[]>([]);
  const [doctors, setDoctors] = useState<Doctor[]>([]);
  
  const [formData, setFormData] = useState({
    patientId: '',
    doctorId: '',
    appointmentDateTime: '',
    reason: '',
    durationMinutes: 30
  });

  useEffect(() => {
    if(!token) return;
    const client = createApiClient(token);
    client.get('/api/patients').then(res => setPatients(res.data));
    client.get('/api/doctors/available').then(res => setDoctors(res.data));
  }, [token]);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!token) return;
    try {
      await createApiClient(token).post('/api/appointments', formData);
      alert('Appointment booked!');
      router.push('/appointments');
    } catch (error) {
      console.error('Failed to book appointment', error);
      alert('Failed to book appointment');
    }
  };

  return (
    <div className="max-w-xl mx-auto mt-10 bg-white p-8 rounded-lg shadow text-black">
      <h2 className="text-2xl font-bold mb-6 text-gray-800">Book Appointment</h2>
      <form onSubmit={handleSubmit} className="space-y-4">
        <div>
            <label className="block text-sm">Patient</label>
            <select name="patientId" className="w-full border p-2 rounded" onChange={e => setFormData({...formData, patientId: e.target.value})} required>
                <option value="">Select Patient</option>
                {patients.map(p => <option key={p.id} value={p.id}>{p.firstName} {p.lastName}</option>)}
            </select>
        </div>
        <div>
            <label className="block text-sm">Doctor</label>
            <select name="doctorId" className="w-full border p-2 rounded" onChange={e => setFormData({...formData, doctorId: e.target.value})} required>
                <option value="">Select Doctor</option>
                {doctors.map(d => <option key={d.id} value={d.id}>Dr. {d.lastName} ({d.specialization})</option>)}
            </select>
        </div>
        <div>
            <label className="block text-sm">Date & Time</label>
            <input type="datetime-local" className="w-full border p-2 rounded" 
                onChange={e => setFormData({...formData, appointmentDateTime: e.target.value})} required />
        </div>
        <div>
            <label className="block text-sm">Reason</label>
            <input type="text" className="w-full border p-2 rounded" 
                onChange={e => setFormData({...formData, reason: e.target.value})} required />
        </div>
        <button type="submit" className="w-full bg-purple-600 text-white py-2 rounded hover:bg-purple-700">Book Now</button>
      </form>
    </div>
  );
}