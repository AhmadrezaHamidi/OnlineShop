import { useAuth } from '../../store/auth'
import { useNavigate } from 'react-router-dom'

export default function PatientProfile() {
  const { user, logout } = useAuth()
  const navigate = useNavigate()

  return (
    <div className="space-y-6 max-w-md">
      <h1 className="text-xl font-bold">پروفایل</h1>
      <div className="card space-y-4">
        <div className="flex items-center gap-4">
          <div className="w-16 h-16 bg-teal-100 rounded-full flex items-center justify-center text-2xl text-teal-600 font-bold">
            {user?.firstName?.[0]}
          </div>
          <div>
            <p className="text-lg font-bold">{user?.firstName} {user?.lastName}</p>
            <p className="text-sm text-gray-500" dir="ltr">{user?.phoneNumber}</p>
          </div>
        </div>
        <hr />
        <div className="grid grid-cols-2 gap-2 text-sm">
          <span className="text-gray-500">نام:</span><span>{user?.firstName}</span>
          <span className="text-gray-500">نام خانوادگی:</span><span>{user?.lastName}</span>
          <span className="text-gray-500">موبایل:</span><span dir="ltr">{user?.phoneNumber}</span>
          <span className="text-gray-500">نقش:</span><span>بیمار</span>
        </div>
        <button className="btn btn-outline w-full text-red-600 border-red-200 hover:bg-red-50"
          onClick={() => { logout(); navigate('/login') }}>
          خروج از حساب
        </button>
      </div>
    </div>
  )
}
