'use client';

import { useEffect, useMemo, useState } from 'react';
import { toast } from 'react-toastify';
import Header from '@/components/Header';
import AppointmentCard from '@/components/AppointmentCard';
import ProtectedRoute from '@/components/ProtectedRoute';
import { useApi } from '@/hooks/useApi';
import { Appointment, Doctor, Patient } from '@/types';

const defaultAppointment: Appointment = {
  patientId: '',
  doctorId: '',
  appointmentDateTime: '',
  reason: '',
  status: 'Scheduled',
  notes: '',
  durationMinutes: 30,
};

export default function AppointmentsPage() {
  const api = useApi();
  const [appointments, setAppointments] = useState<Appointment[]>([]);
  const [patients, setPatients] = useState<Patient[]>([]);
  const [doctors, setDoctors] = useState<Doctor[]>([]);
  const [loading, setLoading] = useState(true);
  const [showForm, setShowForm] = useState(false);
  const [form, setForm] = useState<Appointment>(defaultAppointment);
  const [statusFilter, setStatusFilter] = useState('All');

  useEffect(() => {
    if (!api) return;

    const fetchData = async () => {
      try {
        const [appointmentsRes, patientsRes, doctorsRes] = await Promise.all([
          api.get('/api/appointments'),
          api.get('/api/patients'),
          api.get('/api/doctors'),
        ]);

        setAppointments(appointmentsRes.data);
        setPatients(patientsRes.data);
        setDoctors(doctorsRes.data);
      } catch (error) {
        console.error('Failed to load appointments', error);
        toast.error('Failed to load appointments');
      } finally {
        setLoading(false);
      }
    };

    fetchData();
  }, [api]);

  const filteredAppointments = useMemo(() => {
    if (statusFilter === 'All') return appointments;
    return appointments.filter((appt) => appt.status === statusFilter);
  }, [appointments, statusFilter]);

  const refreshAppointments = async () => {
    if (!api) return;
    const refreshed = await api.get('/api/appointments');
    setAppointments(refreshed.data);
  };

  const handleSubmit = async () => {
    if (!api) return;

    try {
      await api.post('/api/appointments', form);
      toast.success('Appointment created');
      setForm(defaultAppointment);
      setShowForm(false);
      await refreshAppointments();
    } catch (error) {
      console.error('Failed to create appointment', error);
      toast.error('Failed to create appointment');
    }
  };

  const handleComplete = async (appointmentId: string, fee = 100) => {
    if (!api) return;
    try {
      await api.post(`/api/appointments/${appointmentId}/complete`, {
        notes: 'Completed via dashboard',
        consultationFee: fee,
      });
      toast.success('Appointment marked as completed');
      await refreshAppointments();
    } catch (error) {
      console.error('Unable to complete appointment', error);
      toast.error('Unable to complete appointment');
    }
  };

  const handleCancel = async (appointmentId: string) => {
    if (!api) return;

    try {
      await api.post(`/api/appointments/${appointmentId}/cancel`, {
        reason: 'Cancelled by staff',
      });
      toast.success('Appointment cancelled');
      await refreshAppointments();
    } catch (error) {
      console.error('Unable to cancel appointment', error);
      toast.error('Unable to cancel appointment');
    }
  };

  const getPatientName = (patientId: string) => {
    const patient = patients.find((p) => p.id === patientId);
    return patient ? `${patient.firstName} ${patient.lastName}` : 'Unknown patient';
  };

  const getDoctorName = (doctorId: string) => {
    const doctor = doctors.find((d) => d.id === doctorId);
    return doctor ? `Dr. ${doctor.firstName} ${doctor.lastName}` : 'Unknown doctor';
  };

  if (loading || !api) {
    return <div className="card">Loading appointments...</div>;
  }

  return (
    <ProtectedRoute roles={['admin', 'reception']}>
      <Header
        title="Appointments"
        description="Schedule and track upcoming visits."
        actions={
          <button className="btn btn-primary" onClick={() => setShowForm((prev) => !prev)}>
            {showForm ? 'Close Form' : 'Book Appointment'}
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
            <div>
              <label className="label">Patient</label>
              <select
                className="input"
                required
                value={form.patientId}
                onChange={(event) => setForm({ ...form, patientId: event.target.value })}
              >
                <option value="">Select patient</option>
                {patients.map((patient) => (
                  <option key={patient.id} value={patient.id}>
                    {patient.firstName} {patient.lastName}
                  </option>
                ))}
              </select>
            </div>
            <div>
              <label className="label">Doctor</label>
              <select
                className="input"
                required
                value={form.doctorId}
                onChange={(event) => setForm({ ...form, doctorId: event.target.value })}
              >
                <option value="">Select doctor</option>
                {doctors
                  .filter((doctor) => doctor.isAvailable)
                  .map((doctor) => (
                    <option key={doctor.id} value={doctor.id}>
                      Dr. {doctor.firstName} {doctor.lastName} — {doctor.specialization}
                    </option>
                  ))}
              </select>
            </div>
            <div>
              <label className="label">Date &amp; Time</label>
              <input
                className="input"
                type="datetime-local"
                required
                value={form.appointmentDateTime}
                onChange={(event) => setForm({ ...form, appointmentDateTime: event.target.value })}
              />
            </div>
            <div>
              <label className="label">Duration (minutes)</label>
              <input
                className="input"
                type="number"
                min={15}
                value={form.durationMinutes}
                onChange={(event) => setForm({ ...form, durationMinutes: Number(event.target.value) })}
              />
            </div>
            <div className="md:col-span-2">
              <label className="label">Reason</label>
              <textarea
                className="input"
                rows={3}
                value={form.reason}
                onChange={(event) => setForm({ ...form, reason: event.target.value })}
                required
              />
            </div>
            <div className="md:col-span-2">
              <button className="btn btn-primary w-full" type="submit">
                Submit
              </button>
            </div>
          </form>
        </div>
      )}

      <div className="card mb-4 flex flex-wrap gap-4 items-center">
        {['All', 'Scheduled', 'Completed', 'Cancelled'].map((status) => (
          <button
            key={status}
            className={`rounded-full px-4 py-1 text-sm ${
              statusFilter === status ? 'bg-blue-600 text-white' : 'bg-gray-200 text-gray-700'
            }`}
            onClick={() => setStatusFilter(status)}
          >
            {status}
          </button>
        ))}
      </div>

      <div className="grid grid-cols-1 gap-4 md:grid-cols-2">
        {filteredAppointments.map((appointment) => (
          <AppointmentCard
            key={appointment.id}
            appointment={appointment}
            patientName={getPatientName(appointment.patientId)}
            doctorName={getDoctorName(appointment.doctorId)}
            onComplete={(id) => handleComplete(id)}
            onCancel={(id) => handleCancel(id)}
          />
        ))}
      </div>
    </ProtectedRoute>
  );
}
