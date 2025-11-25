'use client';

import { useEffect, useMemo, useState } from 'react';
import { toast } from 'react-toastify';
import Header from '@/components/Header';
import PatientForm from '@/components/PatientForm';
import ProtectedRoute from '@/components/ProtectedRoute';
import { useApi } from '@/hooks/useApi';
import { Patient } from '@/types';

const defaultPatient: Patient = {
  firstName: '',
  lastName: '',
  email: '',
  phone: '',
  dateOfBirth: '',
  gender: '',
  address: '',
  city: '',
  postalCode: '',
  country: '',
  medicalHistory: '',
  allergies: '',
  bloodType: '',
};

export default function PatientsPage() {
  const api = useApi();
  const [patients, setPatients] = useState<Patient[]>([]);
  const [loading, setLoading] = useState(true);
  const [showForm, setShowForm] = useState(false);
  const [form, setForm] = useState<Patient>(defaultPatient);
  const [filter, setFilter] = useState('');
  const [submitting, setSubmitting] = useState(false);

  useEffect(() => {
    if (!api) return;

    const fetchPatients = async () => {
      try {
        const response = await api.get('/api/patients');
        setPatients(response.data);
      } catch (error) {
        console.error('Failed to fetch patients', error);
        toast.error('Failed to fetch patients');
      } finally {
        setLoading(false);
      }
    };

    fetchPatients();
  }, [api]);

  const filteredPatients = useMemo(() => {
    if (!filter) return patients;
    const term = filter.toLowerCase();
    return patients.filter(
      (patient) =>
        patient.firstName.toLowerCase().includes(term) ||
        patient.lastName.toLowerCase().includes(term) ||
        patient.email.toLowerCase().includes(term),
    );
  }, [patients, filter]);

  const handleSubmit = async () => {
    if (!api) return;
    setSubmitting(true);
    try {
      await api.post('/api/patients', form);
      toast.success('Patient created successfully');
      const refreshed = await api.get('/api/patients');
      setPatients(refreshed.data);
      setShowForm(false);
      setForm(defaultPatient);
    } catch (error) {
      console.error('Failed to create patient', error);
      toast.error('Failed to create patient');
    } finally {
      setSubmitting(false);
    }
  };

  const handleDelete = async (id?: string) => {
    if (!api || !id) return;
    if (!confirm('Are you sure you want to delete this patient?')) return;

    try {
      await api.delete(`/api/patients/${id}`);
      toast.success('Patient deleted successfully');
      setPatients((prev) => prev.filter((p) => p.id !== id));
    } catch (error) {
      console.error('Failed to delete patient', error);
      toast.error('Failed to delete patient');
    }
  };

  if (loading || !api) {
    return <div className="card">Loading patients...</div>;
  }

  return (
    <ProtectedRoute roles={['admin', 'reception']}>
      <Header
        title="Patients"
        description="Manage patient records and onboarding."
        actions={
          <button className="btn btn-primary" onClick={() => setShowForm((prev) => !prev)}>
            {showForm ? 'Close Form' : 'Add Patient'}
          </button>
        }
      />

      {showForm && (
        <div className="card mb-6">
          <PatientForm value={form} onChange={setForm} onSubmit={handleSubmit} submitting={submitting} />
        </div>
      )}

      <div className="card mb-4">
        <input
          className="input"
          placeholder="Search by name or email..."
          value={filter}
          onChange={(event) => setFilter(event.target.value)}
        />
      </div>

      <div className="card overflow-x-auto">
        <table className="min-w-full divide-y divide-gray-200">
          <thead className="bg-gray-50">
            <tr>
              <th className="px-4 py-2 text-left text-xs font-semibold uppercase text-gray-500">Name</th>
              <th className="px-4 py-2 text-left text-xs font-semibold uppercase text-gray-500">Email</th>
              <th className="px-4 py-2 text-left text-xs font-semibold uppercase text-gray-500">Phone</th>
              <th className="px-4 py-2 text-left text-xs font-semibold uppercase text-gray-500">Blood Type</th>
              <th className="px-4 py-2 text-right text-xs font-semibold uppercase text-gray-500">Actions</th>
            </tr>
          </thead>
          <tbody className="bg-white divide-y divide-gray-100">
            {filteredPatients.map((patient) => (
              <tr key={patient.id}>
                <td className="px-4 py-3">
                  <p className="font-semibold text-gray-900">
                    {patient.firstName} {patient.lastName}
                  </p>
                  <p className="text-sm text-gray-500">{patient.city}</p>
                </td>
                <td className="px-4 py-3 text-sm text-gray-700">{patient.email}</td>
                <td className="px-4 py-3 text-sm text-gray-700">{patient.phone}</td>
                <td className="px-4 py-3 text-sm text-gray-700">{patient.bloodType}</td>
                <td className="px-4 py-3 text-right">
                  <button className="text-red-600 hover:text-red-800" onClick={() => handleDelete(patient.id)}>
                    Delete
                  </button>
                </td>
              </tr>
            ))}
            {filteredPatients.length === 0 && (
              <tr>
                <td className="px-4 py-6 text-center text-sm text-gray-500" colSpan={5}>
                  No patients found.
                </td>
              </tr>
            )}
          </tbody>
        </table>
      </div>
    </ProtectedRoute>
  );
}
