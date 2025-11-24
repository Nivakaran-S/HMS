// src/app/doctors/[id]/page.tsx

interface PageProps {
  params: {
    id: string;
  };
}

// You MUST have 'export default' here
export default function DoctorDetailsPage({ params }: PageProps) {
  return (
    <div>
      <h1>Doctor Details</h1>
      <p>Doctor ID: {params.id}</p>
    </div>
  );
}