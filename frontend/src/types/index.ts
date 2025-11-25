export interface Patient {
  id?: string;
  firstName: string;
  lastName: string;
  email: string;
  phone: string;
  dateOfBirth: string;
  gender: string;
  address: string;
  city: string;
  postalCode: string;
  country: string;
  medicalHistory: string;
  allergies: string;
  bloodType: string;
  createdAt?: string;
  updatedAt?: string;
}

export interface Doctor {
  id?: string;
  firstName: string;
  lastName: string;
  email: string;
  phone: string;
  specialization: string;
  licenseNumber: string;
  qualifications: string;
  yearsOfExperience: number;
  biography: string;
  consultationFee: number;
  clinicAddress: string;
  department: string;
  isAvailable: boolean;
  createdAt?: string;
  updatedAt?: string;
}

export interface Appointment {
  id?: string;
  patientId: string;
  doctorId: string;
  appointmentDateTime: string;
  reason: string;
  status: string;
  notes: string;
  durationMinutes: number;
  createdAt?: string;
  updatedAt?: string;
}

export interface Bill {
  id?: string;
  appointmentId: string;
  patientId: string;
  doctorId: string;
  consultationFee: number;
  labCharges: number;
  medicineCharges: number;
  otherCharges: number;
  totalAmount: number;
  discount: number;
  taxAmount: number;
  netAmount: number;
  paymentStatus: string;
  paymentMethod: string;
  paymentDate?: string;
  transactionId: string;
  notes: string;
  billDate: string;
  dueDate: string;
  createdAt?: string;
}