import { http, HttpResponse } from 'msw'

const TODAY = new Date().toISOString()

export const shopOrders = [
  {
    id: 101, status: 'Pending', totalAmount: 480000, createdAt: TODAY,
    customerName: 'علی رضایی', customerPhone: '09121234567', sellerName: 'فروشگاه نیکو',
    items: [{ id: 1, productId: 10, productName: 'غذای گربه', quantity: 2, unitPrice: 190000, totalPrice: 380000 }],
    payments: [],
  },
  {
    id: 102, status: 'Delivered', totalAmount: 350000, createdAt: TODAY,
    customerName: 'مریم احمدی', customerPhone: '09351234567', sellerName: 'پت‌لند',
    items: [{ id: 2, productId: 11, productName: 'شامپو سگ', quantity: 1, unitPrice: 350000, totalPrice: 350000 }],
    payments: [],
  },
  {
    id: 103, status: 'Shipped', totalAmount: 210000, createdAt: TODAY,
    customerName: 'رضا کریمی', customerPhone: '09181234567', sellerName: 'زوپت',
    items: [{ id: 3, productId: 12, productName: 'اسباب‌بازی', quantity: 3, unitPrice: 70000, totalPrice: 210000 }],
    payments: [],
  },
]

export const shopProducts = [
  { id: 10, name: 'غذای گربه رویال کنین', description: 'غذای خشک', price: 190000, categoryId: 1, categoryName: 'غذا', isActive: true, stock: 15, images: [] },
  { id: 11, name: 'شامپو سگ هرباسنس',     description: 'شامپو',    price: 145000, categoryId: 2, categoryName: 'بهداشت', isActive: true, stock: 0, images: [] },
  { id: 12, name: 'اسباب‌بازی موش',        description: 'اسباب',   price: 70000,  categoryId: 3, categoryName: 'اسباب‌بازی', isActive: false, stock: 8, images: [] },
]

export const shopUsers = [
  { id: 1, phoneNumber: '09121234567', firstName: 'علی',  lastName: 'رضایی',  isActive: true,  isSuspended: false, roles: ['Customer'], createdAt: TODAY },
  { id: 2, phoneNumber: '09351234567', firstName: 'مریم', lastName: 'احمدی',  isActive: true,  isSuspended: false, roles: ['Seller'],   createdAt: TODAY },
  { id: 3, phoneNumber: '09181234567', firstName: 'رضا',  lastName: 'کریمی',  isActive: false, isSuspended: false, roles: ['Customer'], createdAt: TODAY },
  { id: 4, phoneNumber: '09101234567', firstName: 'سارا', lastName: 'محمدی',  isActive: true,  isSuspended: true,  roles: ['Customer'], createdAt: TODAY },
]

export const shopHandlers = [
  // Auth
  http.post('/api/v1/Identity/Auth/RequestOtp', () =>
    HttpResponse.json({ message: 'کد OTP (DEV): 123456', devCode: '123456' })
  ),
  http.post('/api/v1/Identity/Auth/VerifyOtp', () =>
    HttpResponse.json({
      token: 'shop-fake-token',
      refreshToken: 'shop-fake-refresh',
      expiresAt: new Date(Date.now() + 3600_000).toISOString(),
      user: { id: 1, phoneNumber: '09121234567', firstName: 'احمد', lastName: 'تست', role: 'Admin' },
    })
  ),
  http.post('/api/v1/Identity/Auth/Logout', () => HttpResponse.json({})),

  // Orders
  http.get('/api/v1/Order/', () => HttpResponse.json(shopOrders)),
  http.get('/api/v1/Order/:id', ({ params }) => {
    const o = shopOrders.find(x => x.id === +params.id)
    return o ? HttpResponse.json(o) : HttpResponse.json({ message: 'not found' }, { status: 404 })
  }),
  http.post('/api/v1/Order/', () => HttpResponse.json({ ...shopOrders[0], id: 200, items: [], status: 'Pending' })),
  http.post('/api/v1/Order/:id/Items', () => HttpResponse.json({})),
  http.delete('/api/v1/Order/:id/Items/:itemId', () => HttpResponse.json({})),
  http.put('/api/v1/Order/:id/Place',   () => HttpResponse.json({})),
  http.put('/api/v1/Order/:id/Confirm', () => HttpResponse.json({})),
  http.put('/api/v1/Order/:id/Ship',    () => HttpResponse.json({})),
  http.put('/api/v1/Order/:id/Deliver', () => HttpResponse.json({})),
  http.put('/api/v1/Order/:id/Cancel',  () => HttpResponse.json({})),

  // Products
  http.get('/api/v1/Product/', () => HttpResponse.json(shopProducts)),
  http.get('/api/v1/Product/Categories', () =>
    HttpResponse.json([
      { id: 1, name: 'غذا' }, { id: 2, name: 'بهداشت' }, { id: 3, name: 'اسباب‌بازی' },
    ])
  ),
  http.put('/api/v1/Product/:id/Activate',   () => HttpResponse.json({})),
  http.put('/api/v1/Product/:id/Deactivate', () => HttpResponse.json({})),

  // Users
  http.get('/api/v1/Identity/Users', () => HttpResponse.json(shopUsers)),
  http.put('/api/v1/Identity/Users/:id/Activate',   () => HttpResponse.json({})),
  http.put('/api/v1/Identity/Users/:id/Deactivate', () => HttpResponse.json({})),
  http.put('/api/v1/Identity/Users/:id/Suspend',    () => HttpResponse.json({})),

  // Discounts
  http.get('/api/v1/Discounts', () =>
    HttpResponse.json([
      { id: 1, code: 'PETSHOP20', type: 'Percentage', value: 20, usedCount: 5, isActive: true },
      { id: 2, code: 'SAVE50K',   type: 'Fixed',       value: 50000, usedCount: 12, isActive: false },
    ])
  ),
  http.post('/api/v1/Discounts',           () => HttpResponse.json({ id: 3, code: 'NEW10', type: 'Percentage', value: 10, usedCount: 0, isActive: true })),
  http.put('/api/v1/Discounts/:id/activate',   () => HttpResponse.json({})),
  http.put('/api/v1/Discounts/:id/deactivate', () => HttpResponse.json({})),

  // BNPL
  http.get('/api/v1/Bnpl/Users/:userId/Contracts', () =>
    HttpResponse.json([
      { id: 1, userId: 1, orderId: 101, totalAmount: 480000, installmentCount: 3, status: 'Active', createdAt: TODAY, installments: [] },
    ])
  ),
  http.put('/api/v1/Bnpl/Contracts/:id/Default', () => HttpResponse.json({})),
  http.put('/api/v1/Bnpl/Contracts/:id/Cancel',  () => HttpResponse.json({})),

  // Marketing
  http.post('/api/v1/Marketing/sms/broadcast', () => HttpResponse.json({ sent: true })),

  // Reports
  http.get('/api/v1/BackOffice/Reports', () =>
    HttpResponse.json([
      { id: 1, type: 'Sales', status: 'Completed', requestedAt: TODAY, completedAt: TODAY, fileUrl: '/reports/1.xlsx' },
      { id: 2, type: 'Users', status: 'Pending',   requestedAt: TODAY },
    ])
  ),
  http.post('/api/v1/BackOffice/Reports', () =>
    HttpResponse.json({ id: 3, type: 'Orders', status: 'Pending', requestedAt: TODAY })
  ),

  // Account
  http.get('/api/v1/Account/Profile', () =>
    HttpResponse.json({ id: 1, phoneNumber: '09121234567', firstName: 'احمد', lastName: 'تست', email: 'test@example.com' })
  ),
  http.put('/api/v1/Account/Profile', () => HttpResponse.json({})),
]
