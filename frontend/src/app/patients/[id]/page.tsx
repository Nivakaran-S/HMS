export default function PatientDetailsPage({ params }: { params: { id: string } }) {
  return (
    <div>
      <h1>Patient Details</h1>
      <p>Patient ID: {params.id}</p>
    </div>
  );
}