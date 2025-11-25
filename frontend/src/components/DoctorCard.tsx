import { Doctor } from '@/types';

type DoctorCardProps = {
  doctor: Doctor;
};

export default function DoctorCard({ doctor }: DoctorCardProps) {
  return (
    <div className="card">
      <div className="flex items-center justify-between">
        <div>
          <p className="text-lg font-semibold text-gray-900">
            Dr. {doctor.firstName} {doctor.lastName}
          </p>
          <p className="text-sm text-gray-600">{doctor.specialization}</p>
        </div>
        <span
          className={`rounded-full px-3 py-1 text-xs font-semibold ${
            doctor.isAvailable ? 'bg-green-100 text-green-800' : 'bg-red-100 text-red-800'
          }`}
        >
          {doctor.isAvailable ? 'Available' : 'Unavailable'}
        </span>
      </div>
      <p className="mt-2 text-sm text-gray-600">{doctor.department}</p>
      <p className="mt-4 text-gray-700">{doctor.biography}</p>
      <p className="mt-4 text-sm text-gray-500">Consultation Fee</p>
      <p className="text-xl font-semibold text-blue-600">${doctor.consultationFee}</p>
    </div>
  );
}

