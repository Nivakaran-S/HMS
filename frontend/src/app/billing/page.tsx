'use client';

import { useEffect, useState } from 'react';
import { format } from 'date-fns';
import { toast } from 'react-toastify';
import Header from '@/components/Header';
import ProtectedRoute from '@/components/ProtectedRoute';
import { useApi } from '@/hooks/useApi';
import { Bill } from '@/types';

export default function BillingPage() {
  const api = useApi();
  const [bills, setBills] = useState<Bill[]>([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    if (!api) return;

    const fetchBills = async () => {
      try {
        const response = await api.get('/api/billing');
        setBills(response.data);
      } catch (error) {
        console.error('Failed to fetch bills', error);
        toast.error('Failed to fetch bills');
      } finally {
        setLoading(false);
      }
    };

    fetchBills();
  }, [api]);

  const handlePayment = async (billId?: string) => {
    if (!api || !billId) return;

    try {
      await api.post(`/api/billing/${billId}/payment`, {
        paymentMethod: 'Credit Card',
        transactionId: `TXN${Date.now()}`,
        paymentDate: new Date().toISOString(),
      });
      toast.success('Payment processed successfully');
      const refreshed = await api.get('/api/billing');
      setBills(refreshed.data);
    } catch (error) {
      console.error('Failed to process payment', error);
      toast.error('Failed to process payment');
    }
  };

  if (loading || !api) {
    return <div className="card">Loading billing data...</div>;
  }

  return (
    <ProtectedRoute roles={['admin', 'billing']}>
      <Header title="Billing" description="Track outstanding and completed invoices." />

      <div className="card overflow-x-auto">
        <table className="min-w-full divide-y divide-gray-200">
          <thead className="bg-gray-50">
            <tr>
              <th className="px-6 py-3 text-left text-xs font-medium uppercase text-gray-500">Bill Date</th>
              <th className="px-6 py-3 text-left text-xs font-medium uppercase text-gray-500">Consultation Fee</th>
              <th className="px-6 py-3 text-left text-xs font-medium uppercase text-gray-500">Total</th>
              <th className="px-6 py-3 text-left text-xs font-medium uppercase text-gray-500">Tax</th>
              <th className="px-6 py-3 text-left text-xs font-medium uppercase text-gray-500">Net Amount</th>
              <th className="px-6 py-3 text-left text-xs font-medium uppercase text-gray-500">Status</th>
              <th className="px-6 py-3 text-left text-xs font-medium uppercase text-gray-500">Actions</th>
            </tr>
          </thead>
          <tbody className="bg-white divide-y divide-gray-100">
            {bills.map((bill) => (
              <tr key={bill.id}>
                <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-700">
                  {format(new Date(bill.billDate), 'PP')}
                </td>
                <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-700">
                  ${bill.consultationFee.toFixed(2)}
                </td>
                <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-700">${bill.totalAmount.toFixed(2)}</td>
                <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-700">${bill.taxAmount.toFixed(2)}</td>
                <td className="px-6 py-4 whitespace-nowrap font-semibold text-gray-900">${bill.netAmount.toFixed(2)}</td>
                <td className="px-6 py-4 whitespace-nowrap">
                  <span
                    className={`rounded-full px-3 py-1 text-xs font-semibold ${
                      bill.paymentStatus === 'Paid'
                        ? 'bg-green-100 text-green-800'
                        : bill.paymentStatus === 'Cancelled'
                          ? 'bg-red-100 text-red-800'
                          : 'bg-yellow-100 text-yellow-800'
                    }`}
                  >
                    {bill.paymentStatus}
                  </span>
                </td>
                <td className="px-6 py-4 whitespace-nowrap">
                  {bill.paymentStatus === 'Pending' ? (
                    <button className="btn btn-success btn-sm" onClick={() => handlePayment(bill.id)}>
                      Pay Now
                    </button>
                  ) : (
                    bill.paymentDate && (
                      <span className="text-xs text-gray-500">Paid on {format(new Date(bill.paymentDate), 'PP')}</span>
                    )
                  )}
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>
    </ProtectedRoute>
  );
}
