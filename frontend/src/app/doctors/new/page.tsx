'use client';

import { ChangeEvent, useState } from 'react';
import { useRouter } from 'next/navigation';
import { useAuth } from '@/components/AuthProvider';
import { createApiClient } from '@/services/api';

export default function NewDoctorPage() {
  const { token } = useAuth();
  const router = useRouter();
  const [formData, setFormData] = useState({
    firstName: '',
    lastName: '',
    email: '',
    phone: '',
    specialization: 'General',
    licenseNumber: '',
    consultationFee: 50,
    clinicAddress: '',
    isAvailable: true
  });

  const handleChange = (event: ChangeEvent<HTMLInputElement>) => {
    const { name, type, value, checked } = event.target;
    const nextValue = type === 'checkbox' ? checked : value;
    setFormData({ ...formData, [name]: nextValue });
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!token) return;
    try {
      await createApiClient(token).post('/api/doctors', formData);
      alert('Doctor added successfully!');
      router.push('/doctors');
    } catch (error) {
      console.error(error);
      alert('Failed to add doctor.');
    }
  };

  return (
    <div className="max-w-2xl mx-auto mt-10 bg-white p-8 rounded-lg shadow">
      <h2 className="text-2xl font-bold mb-6 text-gray-800">Onboard New Doctor</h2>
      <form onSubmit={handleSubmit} className="space-y-4">
        <div className="grid grid-cols-2 gap-4">
          <input required name="firstName" placeholder="First Name" onChange={handleChange} className="p-2 border rounded text-black" />
          <input required name="lastName" placeholder="Last Name" onChange={handleChange} className="p-2 border rounded text-black" />
        </div>
        <input required name="email" type="email" placeholder="Email" onChange={handleChange} className="w-full p-2 border rounded text-black" />
        <div className="grid grid-cols-2 gap-4">
            <input required name="phone" placeholder="Phone" onChange={handleChange} className="p-2 border rounded text-black" />
            <input required name="specialization" placeholder="Specialization" onChange={handleChange} className="p-2 border rounded text-black" />
        </div>
        <div className="grid grid-cols-2 gap-4">
            <input required name="licenseNumber" placeholder="License Number" onChange={handleChange} className="p-2 border rounded text-black" />
            <input required name="consultationFee" type="number" placeholder="Fee" onChange={handleChange} className="p-2 border rounded text-black" />
        </div>
        <input name="clinicAddress" placeholder="Clinic Address" onChange={handleChange} className="w-full p-2 border rounded text-black" />
        
        <button type="submit" className="w-full bg-green-600 text-white py-2 px-4 rounded hover:bg-green-700 transition">
          Add Doctor
        </button>
      </form>
    </div>
  );
}