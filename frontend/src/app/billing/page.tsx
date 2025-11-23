'use client';
import { useEffect, useState } from 'react';
import { useAuth } from '@/components/AuthProvider';
import { createApiClient } from '@/services/api';

export default function BillingPage() {
  const { token } = useAuth();
  const [bills, setBills] = useState<any[]>([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    if (token) {
      createApiClient(token).get('/api/billing')
        .then(res => setBills(res.data))
        .catch(err => console.error(err))
        .finally(() => setLoading(false));
    }
  }, [token]);

  if (loading) return <div className="p-6 text-black">Loading billing info...</div>;

  return (
    <div className="container mx-auto p-6">
      <h1 className="text-3xl font-bold text-gray-800 mb-6">Invoices & Billing</h1>
      <div className="grid gap-4">
        {bills.map((bill) => (
          <div key={bill.id} className="bg-white p-6 rounded-lg shadow flex justify-between items-center">
            <div>
              <p className="text-sm text-gray-500">Invoice #{bill.id.substring(0, 8)}</p>
              <p className="text-lg font-bold text-gray-800">${bill.totalAmount.toFixed(2)}</p>
              <p className="text-sm text-gray-600">Date: {new Date(bill.billDate).toLocaleDateString()}</p>
            </div>
            <div>
              <span className={`px-3 py-1 rounded-full text-sm font-medium 
                ${bill.paymentStatus === 'Paid' ? 'bg-green-100 text-green-800' : 'bg-yellow-100 text-yellow-800'}`}>
                {bill.paymentStatus}
              </span>
            </div>
          </div>
        ))}
      </div>
    </div>
  );
}