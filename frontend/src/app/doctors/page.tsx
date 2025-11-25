'use client';

import { useEffect, useState } from 'react';
import { toast } from 'react-toastify';
import Header from '@/components/Header';
import DoctorCard from '@/components/DoctorCard';
import ProtectedRoute from '@/components/ProtectedRoute';
import { useApi } from '@/hooks/useApi';
import { Doctor } from '@/types';

const defaultDoctor: Doctor = {
  firstName: '',
  lastName: '',
  email: '',
  phone: '',
  specialization: '',
  licenseNumber: '',
  qualifications: '',
  yearsOfExperience: 0,
  biography: '',
  consultationFee: 0,
  clinicAddress: '',
  department: '',
  isAvailable: true,
};

type DoctorTextField =
  | 'firstName'
  | 'lastName'
  | 'email'
  | 'phone'
  | 'specialization'
  | 'licenseNumber'
  | 'department'
  | 'clinicAddress';

const textInputs: Array<{ label: string; field: DoctorTextField; type: string }> = [
  { label: 'First Name', field: 'firstName', type: 'text' },
  { label: 'Last Name', field: 'lastName', type: 'text' },
  { label: 'Email', field: 'email', type: 'email' },
  { label: 'Phone', field: 'phone', type: 'tel' },
  { label: 'Specialization', field: 'specialization', type: 'text' },
  { label: 'License Number', field: 'licenseNumber', type: 'text' },
  { label: 'Department', field: 'department', type: 'text' },
  { label: 'Clinic Address', field: 'clinicAddress', type: 'text' },
];

export default function DoctorsPage() {
  const api = useApi();
  const [doctors, setDoctors] = useState<Doctor[]>([]);
  const [loading, setLoading] = useState(true);
  const [showForm, setShowForm] = useState(false);
  const [form, setForm] = useState<Doctor>(defaultDoctor);

  useEffect(() => {
    if (!api) return;

    const fetchDoctors = async () => {
      try {
        const response = await api.get('/api/doctors');
        setDoctors(response.data);
      } catch (error) {
        console.error('Failed to fetch doctors', error);
        toast.error('Failed to fetch doctors');
      } finally {
        setLoading(false);
      }
    };

    fetchDoctors();
  }, [api]);

  const handleSubmit = async () => {
    if (!api) return;

    try {
      await api.post('/api/doctors', form);
      toast.success('Doctor created successfully');
      const refreshed = await api.get('/api/doctors');
      setDoctors(refreshed.data);
      setForm(defaultDoctor);
      setShowForm(false);
    } catch (error) {
      console.error('Failed to create doctor', error);
      toast.error('Failed to create doctor');
    }
  };

  if (loading || !api) {
    return <div className="card">Loading doctors...</div>;
  }

  return (
    <ProtectedRoute roles={['admin']}>
      <Header
        title="Doctors"
        description="Manage provider roster and availability."
        actions={
          <button className="btn btn-primary" onClick={() => setShowForm((prev) => !prev)}>
            {showForm ? 'Close Form' : 'Add Doctor'}
          </button>
        }
      />

      {showForm && (
        <div className="card mb-6">
          <form
            onSubmit={(event) => {
              event.preventDefault();
              handleSubmit();
            }}
            className="grid grid-cols-1 gap-4 md:grid-cols-2"
          >
            {textInputs.map((input) => (
              <div key={input.field}>
                <label className="label">{input.label}</label>
                <input
                  className="input"
                  type={input.type}
                  value={form[input.field] ?? ''}
                  onChange={(event) => setForm({ ...form, [input.field]: event.target.value })}
                  required
                />
              </div>
            ))}
            <div>
              <label className="label">Years of Experience</label>
              <input
                className="input"
                type="number"
                min={0}
                value={form.yearsOfExperience}
                onChange={(event) => setForm({ ...form, yearsOfExperience: Number(event.target.value) })}
              />
            </div>
            <div>
              <label className="label">Consultation Fee</label>
              <input
                className="input"
                type="number"
                min={0}
                value={form.consultationFee}
                onChange={(event) => setForm({ ...form, consultationFee: Number(event.target.value) })}
              />
            </div>
            <div className="md:col-span-2">
              <label className="label">Biography</label>
              <textarea
                className="input"
                rows={3}
                value={form.biography}
                onChange={(event) => setForm({ ...form, biography: event.target.value })}
              />
            </div>
            <div className="md:col-span-2">
              <label className="label">Qualifications</label>
              <textarea
                className="input"
                rows={2}
                value={form.qualifications}
                onChange={(event) => setForm({ ...form, qualifications: event.target.value })}
              />
            </div>
            <div className="md:col-span-2 flex items-center gap-3">
              <input
                id="isAvailable"
                type="checkbox"
                checked={form.isAvailable}
                onChange={(event) => setForm({ ...form, isAvailable: event.target.checked })}
              />
              <label htmlFor="isAvailable" className="text-sm text-gray-700">
                Available for appointments
              </label>
            </div>
            <div className="md:col-span-2">
              <button className="btn btn-primary w-full" type="submit">
                Save Doctor
              </button>
            </div>
          </form>
        </div>
      )}

      <div className="grid grid-cols-1 gap-6 md:grid-cols-2 lg:grid-cols-3">
        {doctors.map((doctor) => (
          <DoctorCard doctor={doctor} key={doctor.id} />
        ))}
      </div>
    </ProtectedRoute>
  );
}
