import { useShopAuth } from '../../../store/shopAuth'

export default function CustomerHome() {
  const { user } = useShopAuth()
  return (
    <div className="space-y-5">
      {/* hero */}
      <div className="bg-gradient-to-br from-pet-500 to-pet-600 rounded-3xl p-6 text-white relative overflow-hidden">
        <div className="absolute left-4 top-1/2 -translate-y-1/2 text-6xl opacity-20 select-none">🐾</div>
        <p className="text-white/80 text-sm mb-1">سلام {user?.firstName ?? '👋'}</p>
        <h2 className="text-2xl font-extrabold leading-tight">بهترین برای<br/>حیوان خانگیت</h2>
        <p className="text-white/70 text-xs mt-2">تخفیف ویژه تا ۴۰٪ 🎉</p>
      </div>

      {/* search */}
      <div className="bg-white rounded-2xl border border-pet-100 px-4 py-3 flex items-center gap-3 shadow-sm">
        <span className="text-xl">🔍</span>
        <input
          placeholder="جستجو در محصولات..."
          className="flex-1 bg-transparent outline-none text-sm text-ink-900 placeholder:text-ink-300"
          readOnly
        />
      </div>

      {/* categories */}
      <div>
        <h3 className="font-bold text-ink-900 mb-3">دسته‌بندی‌ها</h3>
        <div className="flex gap-3 overflow-x-auto pb-1 scrollbar-none">
          {[
            { icon: '🦴', label: 'غذا' },
            { icon: '🐶', label: 'سگ' },
            { icon: '🐱', label: 'گربه' },
            { icon: '🦜', label: 'پرنده' },
            { icon: '🧴', label: 'بهداشت' },
            { icon: '🎾', label: 'اسباب‌بازی' },
          ].map(c => (
            <button key={c.label} className="flex-shrink-0 flex flex-col items-center gap-1 bg-white border border-pet-100 rounded-2xl px-4 py-3 text-xs font-semibold text-ink-500 hover:border-pet-300 hover:text-pet-500 transition-all shadow-sm">
              <span className="text-2xl">{c.icon}</span>
              {c.label}
            </button>
          ))}
        </div>
      </div>

      {/* products grid */}
      <div>
        <h3 className="font-bold text-ink-900 mb-3">پیشنهاد ویژه 🔥</h3>
        <div className="grid grid-cols-2 gap-3">
          {[
            { icon: '🦴', name: 'غذای خشک Royal Canin', price: '۲۸۵,۰۰۰', old: '۳۵۰,۰۰۰', off: '۲۰٪' },
            { icon: '🐟', name: 'غذای مرطوب Whiskas', price: '۴۵,۰۰۰', old: '', off: '' },
            { icon: '🎾', name: 'توپ ربری سگ', price: '۸۵,۰۰۰', old: '۱۰۰,۰۰۰', off: '۱۵٪' },
            { icon: '🧴', name: 'شامپو Bio Groom', price: '۱۲۰,۰۰۰', old: '', off: '' },
          ].map(p => (
            <div key={p.name} className="bg-white rounded-2xl border border-pet-100 overflow-hidden shadow-sm hover:shadow-pet transition-all cursor-pointer">
              <div className="h-24 bg-pet-50 flex items-center justify-center text-4xl relative">
                {p.icon}
                {p.off && (
                  <span className="absolute top-2 left-2 bg-red-500 text-white text-[10px] font-bold px-2 py-0.5 rounded-lg">
                    {p.off}
                  </span>
                )}
              </div>
              <div className="p-3">
                <p className="text-xs font-semibold text-ink-900 leading-snug mb-2">{p.name}</p>
                <div className="flex items-center justify-between">
                  <div>
                    <p className="text-sm font-extrabold text-pet-500">{p.price} ت</p>
                    {p.old && <p className="text-[10px] text-ink-300 line-through">{p.old}</p>}
                  </div>
                  <button className="w-8 h-8 bg-pet-500 text-white rounded-xl flex items-center justify-center text-lg font-bold shadow-pet hover:bg-pet-600 transition-all active:scale-95">
                    +
                  </button>
                </div>
              </div>
            </div>
          ))}
        </div>
      </div>
    </div>
  )
}
