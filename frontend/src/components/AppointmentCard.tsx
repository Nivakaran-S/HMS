import { Appointment } from '@/types';
import { format } from 'date-fns';

type AppointmentCardProps = {
  appointment: Appointment;
  patientName: string;
  doctorName: string;
  onComplete?: (id: string) => void;
  onCancel?: (id: string) => void;
};

export default function AppointmentCard({
  appointment,
  patientName,
  doctorName,
  onComplete,
  onCancel,
}: AppointmentCardProps) {
  const isScheduled = appointment.status === 'Scheduled';

  return (
    <div className="rounded-lg border border-gray-200 bg-white p-4 shadow-sm">
      <div className="flex items-center justify-between">
        <div>
          <p className="text-sm text-gray-500">{format(new Date(appointment.appointmentDateTime), 'PPpp')}</p>
          <p className="text-lg font-semibold text-gray-900">{patientName}</p>
          <p className="text-sm text-gray-600">{doctorName}</p>
        </div>
        <span
          className={`rounded-full px-3 py-1 text-xs font-semibold ${
            appointment.status === 'Completed'
              ? 'bg-green-100 text-green-800'
              : appointment.status === 'Cancelled'
                ? 'bg-red-100 text-red-800'
                : 'bg-blue-100 text-blue-800'
          }`}
        >
          {appointment.status}
        </span>
      </div>

      <p className="mt-3 text-sm text-gray-700">{appointment.reason}</p>

      {isScheduled && (
        <div className="mt-4 flex gap-2">
          {onComplete && (
            <button className="btn btn-success flex-1" onClick={() => onComplete(appointment.id!)}>
              Complete
            </button>
          )}
          {onCancel && (
            <button className="btn btn-danger flex-1" onClick={() => onCancel(appointment.id!)}>
              Cancel
            </button>
          )}
        </div>
      )}
    </div>
  );
}

