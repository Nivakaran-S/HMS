import { Patient } from '@/types';

type PatientFormProps = {
  value: Patient;
  onChange: (patient: Patient) => void;
  onSubmit: () => void;
  submitting?: boolean;
};

export default function PatientForm({ value, onChange, onSubmit, submitting }: PatientFormProps) {
  const updateField = (field: keyof Patient, fieldValue: string) => {
    onChange({ ...value, [field]: fieldValue });
  };

  return (
    <form
      onSubmit={(event) => {
        event.preventDefault();
        onSubmit();
      }}
      className="grid grid-cols-1 gap-4 md:grid-cols-2"
    >
      <div>
        <label className="label">First Name</label>
        <input className="input" required value={value.firstName} onChange={(e) => updateField('firstName', e.target.value)} />
      </div>
      <div>
        <label className="label">Last Name</label>
        <input className="input" required value={value.lastName} onChange={(e) => updateField('lastName', e.target.value)} />
      </div>
      <div>
        <label className="label">Email</label>
        <input
          className="input"
          type="email"
          required
          value={value.email}
          onChange={(e) => updateField('email', e.target.value)}
        />
      </div>
      <div>
        <label className="label">Phone</label>
        <input className="input" required value={value.phone} onChange={(e) => updateField('phone', e.target.value)} />
      </div>
      <div>
        <label className="label">Date of Birth</label>
        <input
          className="input"
          type="date"
          required
          value={value.dateOfBirth ? value.dateOfBirth.substring(0, 10) : ''}
          onChange={(e) => updateField('dateOfBirth', e.target.value)}
        />
      </div>
      <div>
        <label className="label">Gender</label>
        <select className="input" value={value.gender} onChange={(e) => updateField('gender', e.target.value)}>
          <option value="">Select Gender</option>
          <option value="Male">Male</option>
          <option value="Female">Female</option>
          <option value="Other">Other</option>
        </select>
      </div>
      <div className="md:col-span-2">
        <label className="label">Address</label>
        <input className="input" value={value.address} onChange={(e) => updateField('address', e.target.value)} />
      </div>
      <div className="md:col-span-2">
        <label className="label">Medical History</label>
        <textarea className="input" rows={3} value={value.medicalHistory} onChange={(e) => updateField('medicalHistory', e.target.value)} />
      </div>
      <div className="md:col-span-2">
        <button type="submit" className="btn btn-primary w-full" disabled={submitting}>
          {submitting ? 'Saving...' : 'Save Patient'}
        </button>
      </div>
    </form>
  );
}
