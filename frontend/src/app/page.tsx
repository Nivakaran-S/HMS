'use client';

import Link from "next/link";

export default function Home() {
  return (
    <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6 mt-10">
      <DashboardCard 
        title="Patients" 
        description="Manage patient records and history."
        link="/patients"
        color="bg-blue-500"
      />
      <DashboardCard 
        title="Doctors" 
        description="View available doctors and specializations."
        link="/doctors"
        color="bg-green-500"
      />
      <DashboardCard 
        title="Appointments" 
        description="Schedule and manage visits."
        link="/appointments"
        color="bg-purple-500"
      />
      <DashboardCard 
        title="Billing" 
        description="Process payments and view invoices."
        link="/billing"
        color="bg-orange-500"
      />
    </div>
  );
}

function DashboardCard({ title, description, link, color }: any) {
  return (
    <Link href={link} className={`block p-6 rounded-lg shadow-lg text-white ${color} hover:opacity-90 transition`}>
      <h2 className="text-2xl font-bold mb-2">{title}</h2>
      <p>{description}</p>
    </Link>
  );
}