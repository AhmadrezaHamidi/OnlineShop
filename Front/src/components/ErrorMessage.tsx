export default function ErrorMessage({ message }: { message?: string }) {
  return (
    <div className="bg-red-50 border border-red-200 text-red-700 rounded-lg p-4 text-sm">
      {message ?? 'خطایی رخ داد'}
    </div>
  )
}
