import { useState } from 'react'
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { useNavigate } from 'react-router-dom'
import { ordersApi, type Order } from '../../../api/shop/orders'

export default function CustomerCart() {
  const [discountCode, setDiscountCode] = useState('')
  const [placing, setPlacing]           = useState(false)
  const navigate = useNavigate()
  const qc = useQueryClient()

  // سبد = سفارش‌های Pending که هنوز Place نشدن
  const { data: orders = [], isLoading } = useQuery({
    queryKey: ['orders'],
    queryFn: ordersApi.list,
  })

  const cartOrder: Order | undefined = orders.find(
    (o: Order) => o.status === 'Pending' && o.items.length > 0
  )

  const removeItemMut = useMutation({
    mutationFn: ({ orderId, itemId }: { orderId: number; itemId: number }) =>
      ordersApi.removeItem(orderId, itemId),
    onSuccess: () => qc.invalidateQueries({ queryKey: ['orders'] }),
  })

  const placeMut = useMutation({
    mutationFn: (orderId: number) => ordersApi.place(orderId),
    onSuccess: () => {
      qc.invalidateQueries({ queryKey: ['orders'] })
      navigate('/shop/customer/orders')
    },
  })

  if (isLoading) return (
    <div className="flex justify-center py-20 text-ink-300">
      <span className="text-3xl animate-spin">⏳</span>
    </div>
  )

  if (!cartOrder || cartOrder.items.length === 0) return (
    <div className="text-center py-20 space-y-4">
      <div className="text-6xl">🛒</div>
      <p className="text-ink-400 font-semibold">سبد خرید خالی است</p>
      <button
        onClick={() => navigate('/shop/customer/products')}
        className="px-6 py-2.5 rounded-xl bg-pet-500 text-white font-semibold text-sm hover:bg-pet-600 transition-colors"
      >
        مشاهده محصولات
      </button>
    </div>
  )

  const total = cartOrder.items.reduce((s, i) => s + i.totalPrice, 0)

  return (
    <div className="space-y-4">
      <h1 className="text-xl font-extrabold text-ink-900">سبد خرید</h1>

      <div className="space-y-3">
        {cartOrder.items.map(item => (
          <div key={item.id} className="bg-white rounded-2xl border border-ink-100 shadow-sm p-4 flex items-center gap-3">
            <div className="w-14 h-14 bg-pet-50 rounded-xl flex items-center justify-center text-3xl shrink-0">
              🛍️
            </div>
            <div className="flex-1 min-w-0">
              <p className="font-bold text-ink-900 text-sm truncate">{item.productName}</p>
              <p className="text-xs text-ink-300 mt-0.5">تعداد: {item.quantity}</p>
              <p className="font-extrabold text-pet-500 text-sm mt-1">
                {item.totalPrice.toLocaleString('fa-IR')} تومان
              </p>
            </div>
            <button
              onClick={() => removeItemMut.mutate({ orderId: cartOrder.id, itemId: item.id })}
              disabled={removeItemMut.isPending}
              className="w-8 h-8 rounded-xl border border-red-100 text-red-400 hover:bg-red-50 flex items-center justify-center text-sm transition-colors disabled:opacity-40"
            >
              🗑️
            </button>
          </div>
        ))}
      </div>

      {/* کد تخفیف */}
      <div className="flex gap-2">
        <input
          value={discountCode}
          onChange={e => setDiscountCode(e.target.value)}
          placeholder="کد تخفیف دارید؟"
          className="flex-1 px-4 py-2.5 rounded-xl border border-ink-100 bg-white text-sm outline-none focus:border-pet-400 transition-all"
          dir="rtl"
        />
        <button className="px-4 py-2.5 rounded-xl border border-pet-300 text-pet-500 text-sm font-semibold hover:bg-pet-50 transition-colors">
          اعمال
        </button>
      </div>

      {/* جمع */}
      <div className="bg-white rounded-2xl border border-ink-100 shadow-sm p-4 space-y-3">
        <div className="flex justify-between text-sm">
          <span className="text-ink-400">جمع اقلام</span>
          <span className="font-semibold text-ink-900">{total.toLocaleString('fa-IR')} تومان</span>
        </div>
        <div className="flex justify-between text-sm">
          <span className="text-ink-400">هزینه ارسال</span>
          <span className="font-semibold text-forest-500">رایگان</span>
        </div>
        <div className="border-t border-ink-100 pt-3 flex justify-between">
          <span className="font-extrabold text-ink-900">مبلغ نهایی</span>
          <span className="font-extrabold text-pet-500 text-lg">{total.toLocaleString('fa-IR')} تومان</span>
        </div>
      </div>

      <button
        onClick={() => placeMut.mutate(cartOrder.id)}
        disabled={placeMut.isPending}
        className="w-full py-3.5 rounded-2xl bg-pet-500 text-white font-extrabold text-base hover:bg-pet-600 active:scale-[.98] transition-all disabled:opacity-50"
      >
        {placeMut.isPending ? 'در حال ثبت…' : 'ثبت سفارش و پرداخت'}
      </button>
    </div>
  )
}
