import { useState } from 'react'
import { useQuery } from '@tanstack/react-query'
import { useNavigate } from 'react-router-dom'
import { productsApi, type Product } from '../../../api/shop/products'
import { ordersApi } from '../../../api/shop/orders'
import { useMutation, useQueryClient } from '@tanstack/react-query'

export default function CustomerProducts() {
  const [categoryId, setCategoryId] = useState<number | undefined>()
  const [search, setSearch]         = useState('')
  const [cartMsg, setCartMsg]       = useState('')
  const navigate = useNavigate()
  const qc = useQueryClient()

  const { data: categories = [] } = useQuery({
    queryKey: ['categories'],
    queryFn: productsApi.categories,
  })

  const { data: products = [], isLoading } = useQuery({
    queryKey: ['products', categoryId],
    queryFn: () => productsApi.list(categoryId),
  })

  const addToCartMut = useMutation({
    mutationFn: async (productId: number) => {
      const order = await ordersApi.create()
      await ordersApi.addItem(order.id, productId, 1)
      return order.id
    },
    onSuccess: (orderId) => {
      qc.invalidateQueries({ queryKey: ['orders'] })
      setCartMsg('به سبد خرید اضافه شد ✓')
      setTimeout(() => setCartMsg(''), 2000)
    },
  })

  const visible = products.filter((p: Product) =>
    !search || p.name.includes(search) || p.description?.includes(search)
  )

  return (
    <div className="space-y-4">
      <h1 className="text-xl font-extrabold text-ink-900">محصولات</h1>

      <input
        value={search}
        onChange={e => setSearch(e.target.value)}
        placeholder="جستجو در محصولات…"
        className="w-full px-4 py-2.5 rounded-xl border border-ink-100 bg-white text-sm outline-none focus:border-pet-400 focus:ring-2 focus:ring-pet-100 transition-all"
        dir="rtl"
      />

      <div className="flex gap-2 overflow-x-auto pb-1 no-scrollbar">
        <button
          onClick={() => setCategoryId(undefined)}
          className={`shrink-0 px-3 py-1.5 rounded-full text-xs font-semibold border transition-all ${
            !categoryId ? 'bg-pet-500 text-white border-pet-500' : 'bg-white text-ink-500 border-ink-100'
          }`}
        >
          همه
        </button>
        {categories.map((c: { id: number; name: string }) => (
          <button
            key={c.id}
            onClick={() => setCategoryId(c.id)}
            className={`shrink-0 px-3 py-1.5 rounded-full text-xs font-semibold border transition-all ${
              categoryId === c.id ? 'bg-pet-500 text-white border-pet-500' : 'bg-white text-ink-500 border-ink-100'
            }`}
          >
            {c.name}
          </button>
        ))}
      </div>

      {cartMsg && (
        <div className="bg-forest-50 text-forest-600 text-sm font-semibold text-center py-2.5 rounded-xl">
          {cartMsg}
        </div>
      )}

      {isLoading ? (
        <div className="flex justify-center py-16 text-ink-300">
          <span className="text-3xl animate-spin">⏳</span>
        </div>
      ) : visible.length === 0 ? (
        <div className="text-center py-16 text-ink-300">
          <div className="text-5xl mb-3">🔍</div>
          <p className="text-sm">محصولی یافت نشد</p>
        </div>
      ) : (
        <div className="grid grid-cols-2 gap-3">
          {visible.map((p: Product) => (
            <div key={p.id} className="bg-white rounded-2xl border border-ink-100 shadow-sm overflow-hidden">
              <div className="bg-pet-50 h-28 flex items-center justify-center text-4xl">
                {p.images?.[0]?.url ? (
                  <img src={p.images[0].url} alt={p.name} className="h-full w-full object-cover" />
                ) : '🐾'}
              </div>
              <div className="p-3">
                <p className="font-bold text-ink-900 text-xs leading-snug line-clamp-2">{p.name}</p>
                <p className="text-xs text-ink-300 mt-1">{p.categoryName}</p>
                <div className="mt-2 flex items-center justify-between">
                  <div>
                    {p.discountedPrice ? (
                      <>
                        <p className="text-xs line-through text-ink-300">{p.price.toLocaleString('fa-IR')}</p>
                        <p className="font-extrabold text-pet-500 text-sm">{p.discountedPrice.toLocaleString('fa-IR')}</p>
                      </>
                    ) : (
                      <p className="font-extrabold text-ink-900 text-sm">{p.price.toLocaleString('fa-IR')}</p>
                    )}
                    <p className="text-[10px] text-ink-300">تومان</p>
                  </div>
                  <button
                    onClick={() => addToCartMut.mutate(p.id)}
                    disabled={addToCartMut.isPending || p.stock === 0}
                    className="w-8 h-8 rounded-xl bg-pet-500 text-white flex items-center justify-center hover:bg-pet-600 active:scale-95 transition-all disabled:opacity-40"
                  >
                    {p.stock === 0 ? '✗' : '+'}
                  </button>
                </div>
              </div>
            </div>
          ))}
        </div>
      )}
    </div>
  )
}
