import { useQuery } from '@tanstack/react-query'
import { ordersApi } from '../../api/orders'
import { productsApi } from '../../api/products'
import { ORDER_STATUS_LABEL, ORDER_STATUS_PILL } from '../../types'

export default function SellerDashboard() {
  const { data: ordersData, isLoading } = useQuery({
    queryKey: ['seller-orders'],
    queryFn: () => ordersApi.getAll({ page: 1, pageSize: 6 }),
  })
  const { data: productsData } = useQuery({
    queryKey: ['seller-products-count'],
    queryFn: () => productsApi.getAll({ page: 1, pageSize: 1 }),
  })

  const orders = ordersData?.data?.data?.items ?? ordersData?.data?.items ?? []
  const totalOrders = ordersData?.data?.data?.totalCount ?? ordersData?.data?.totalCount ?? 0
  const totalProducts = productsData?.data?.totalCount ?? 0

  return (
    <>
      <div style={{ marginBottom: '1.5rem' }}>
        <h2 style={{ margin: 0, fontSize: '1.4rem', fontWeight: 700 }}>داشبورد فروشنده</h2>
        <p style={{ margin: '4px 0 0', color: 'var(--text2, #64748b)', fontSize: '.9rem' }}>
          {new Date().toLocaleDateString('fa-IR', { weekday: 'long', year: 'numeric', month: 'long', day: 'numeric' })}
        </p>
      </div>

      {/* Stat cards */}
      <div style={{ display: 'grid', gridTemplateColumns: 'repeat(auto-fill,minmax(200px,1fr))', gap: 16, marginBottom: 24 }}>
        <div className="stat-card">
          <div className="icon-wrap" style={{ background: '#EDE9FF' }}>
            <i className="ti ti-shopping-cart" style={{ fontSize: 18, color: '#5C35D4' }} />
          </div>
          <div className="val">{totalOrders.toLocaleString('fa-IR')}</div>
          <div className="lbl">کل سفارش‌ها</div>
        </div>
        <div className="stat-card">
          <div className="icon-wrap" style={{ background: '#CFFAFE' }}>
            <i className="ti ti-package" style={{ fontSize: 18, color: '#0891B2' }} />
          </div>
          <div className="val">{totalProducts.toLocaleString('fa-IR')}</div>
          <div className="lbl">محصولات من</div>
        </div>
        <div className="stat-card">
          <div className="icon-wrap" style={{ background: '#DCFCE7' }}>
            <i className="ti ti-clock-hour-4" style={{ fontSize: 18, color: '#059669' }} />
          </div>
          <div className="val">{orders.filter(o => o.status === 1).length}</div>
          <div className="lbl">منتظر تأیید</div>
        </div>
        <div className="stat-card">
          <div className="icon-wrap" style={{ background: '#FEF3C7' }}>
            <i className="ti ti-truck" style={{ fontSize: 18, color: '#D97706' }} />
          </div>
          <div className="val">{orders.filter(o => o.status === 3).length}</div>
          <div className="lbl">در حال ارسال</div>
        </div>
      </div>

      {/* Recent orders */}
      <div className="card">
        <h3 style={{ margin: '0 0 1rem', fontSize: '1rem', fontWeight: 600 }}>آخرین سفارش‌ها</h3>
        {isLoading ? (
          <div className="spinner-center"><div className="spinner" /></div>
        ) : (
          <table>
            <thead>
              <tr>
                <th>شناسه</th>
                <th>مبلغ</th>
                <th>آیتم‌ها</th>
                <th>وضعیت</th>
                <th>تاریخ</th>
              </tr>
            </thead>
            <tbody>
              {orders.map(o => (
                <tr key={o.id}>
                  <td><span style={{ fontWeight: 700, color: 'var(--p)' }}>#{o.id}</span></td>
                  <td>{o.totalAmount.toLocaleString('fa-IR')} ت</td>
                  <td>{o.items?.length ?? '—'}</td>
                  <td>
                    <span className={`pill ${ORDER_STATUS_PILL[o.status] ?? 'pill-gray'}`}>
                      {ORDER_STATUS_LABEL[o.status] ?? o.status}
                    </span>
                  </td>
                  <td style={{ fontSize: 12, color: 'var(--text2)' }}>
                    {o.placedAt ? new Date(o.placedAt).toLocaleDateString('fa-IR') : '—'}
                  </td>
                </tr>
              ))}
              {orders.length === 0 && (
                <tr><td colSpan={5} style={{ textAlign: 'center', padding: 32, color: '#94a3b8' }}>سفارشی یافت نشد</td></tr>
              )}
            </tbody>
          </table>
        )}
      </div>
    </>
  )
}
