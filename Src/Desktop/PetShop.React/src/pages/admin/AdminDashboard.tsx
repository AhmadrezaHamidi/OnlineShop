import { useQuery } from '@tanstack/react-query'
import { ordersApi } from '../../api/orders'
import { productsApi } from '../../api/products'
import { ORDER_STATUS_LABEL, ORDER_STATUS_PILL } from '../../types'
import styles from './AdminDashboard.module.css'

export default function AdminDashboard() {
  const { data: ordersData, isLoading: loadingOrders } = useQuery({
    queryKey: ['orders', 1, 8],
    queryFn: () => ordersApi.getAll({ page: 1, pageSize: 8 }),
  })
  const { data: productsData } = useQuery({
    queryKey: ['products', 1, 1],
    queryFn: () => productsApi.getAll({ page: 1, pageSize: 1 }),
  })

  const orders = ordersData?.data?.data?.items ?? []
  const totalOrders = ordersData?.data?.data?.totalCount ?? 0
  const totalProducts = productsData?.data?.data?.totalCount ?? 0

  const totalRevenue = orders
    .filter(o => o.status === 4 || o.status === 2)
    .reduce((s, o) => s + o.totalAmount, 0)

  return (
    <>
      <div className={styles.pageHeader}>
        <h2>داشبورد</h2>
        <span className={styles.date}>امروز — {new Date().toLocaleDateString('fa-IR')}</span>
      </div>

      {/* Metrics */}
      <div className={styles.metricsGrid}>
        <div className="stat-card">
          <div className="icon-wrap" style={{ background: '#EDE9FF' }}>
            <i className="ti ti-shopping-cart" style={{ fontSize: 18, color: '#5C35D4' }} />
          </div>
          <div className="val">{totalOrders.toLocaleString('fa-IR')}</div>
          <div className="lbl">کل سفارش‌ها</div>
          <div className="chg chg-up">↑ فعال</div>
        </div>
        <div className="stat-card">
          <div className="icon-wrap" style={{ background: '#CFFAFE' }}>
            <i className="ti ti-cash" style={{ fontSize: 18, color: '#0891B2' }} />
          </div>
          <div className="val">{(totalRevenue / 1_000_000).toFixed(1)}M</div>
          <div className="lbl">درآمد (تومان)</div>
          <div className="chg chg-up">↑ تحویل‌شده‌ها</div>
        </div>
        <div className="stat-card">
          <div className="icon-wrap" style={{ background: '#DCFCE7' }}>
            <i className="ti ti-box" style={{ fontSize: 18, color: '#059669' }} />
          </div>
          <div className="val">{totalProducts.toLocaleString('fa-IR')}</div>
          <div className="lbl">کل محصولات</div>
          <div className="chg chg-up">↑ موجود</div>
        </div>
        <div className="stat-card">
          <div className="icon-wrap" style={{ background: '#FEF3C7' }}>
            <i className="ti ti-clock" style={{ fontSize: 18, color: '#D97706' }} />
          </div>
          <div className="val">
            {orders.filter(o => o.status === 1).length}
          </div>
          <div className="lbl">انتظار تأیید</div>
          <div className="chg" style={{ color: '#D97706' }}>⏳ در صف</div>
        </div>
      </div>

      {/* Recent Orders */}
      <div className="card">
        <div className={styles.tableHeader}>
          <h3>آخرین سفارش‌ها</h3>
        </div>
        {loadingOrders ? (
          <div className="spinner-center"><div className="spinner" /></div>
        ) : (
          <table>
            <thead>
              <tr>
                <th>شناسه</th>
                <th>کاربر</th>
                <th>مبلغ (تومان)</th>
                <th>آیتم‌ها</th>
                <th>وضعیت</th>
                <th>تاریخ</th>
              </tr>
            </thead>
            <tbody>
              {orders.map(order => (
                <tr key={order.id}>
                  <td><span style={{ fontWeight: 700, color: 'var(--p)' }}>#{order.id}</span></td>
                  <td>{order.userId}</td>
                  <td style={{ fontWeight: 700 }}>{order.totalAmount.toLocaleString('fa-IR')}</td>
                  <td>{order.items?.length ?? '—'} آیتم</td>
                  <td>
                    <span className={`pill ${ORDER_STATUS_PILL[order.status] ?? 'pill-gray'}`}>
                      {ORDER_STATUS_LABEL[order.status] ?? order.status}
                    </span>
                  </td>
                  <td style={{ color: 'var(--text2)', fontSize: 12 }}>
                    {order.placedAt ? new Date(order.placedAt).toLocaleDateString('fa-IR') : '—'}
                  </td>
                </tr>
              ))}
              {orders.length === 0 && (
                <tr><td colSpan={6} style={{ textAlign: 'center', color: 'var(--text3)', padding: 32 }}>سفارشی یافت نشد</td></tr>
              )}
            </tbody>
          </table>
        )}
      </div>
    </>
  )
}
