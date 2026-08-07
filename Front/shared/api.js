const BASE = 'http://localhost:80';

export function getToken() {
  return localStorage.getItem('clinic_token');
}
export function setToken(t) {
  localStorage.setItem('clinic_token', t);
}
export function getUser() {
  const raw = localStorage.getItem('clinic_user');
  return raw ? JSON.parse(raw) : null;
}
export function setUser(u) {
  localStorage.setItem('clinic_user', JSON.stringify(u));
}
export function logout() {
  localStorage.removeItem('clinic_token');
  localStorage.removeItem('clinic_user');
}

export async function api(method, path, body = null) {
  const token = getToken();
  const opts = {
    method,
    headers: { 'Content-Type': 'application/json' },
  };
  if (token) opts.headers['Authorization'] = `Bearer ${token}`;
  if (body) opts.body = JSON.stringify(body);

  const res = await fetch(BASE + path, opts);
  if (res.status === 204) return null;
  const data = await res.json().catch(() => null);
  if (!res.ok) throw data ?? { message: 'خطا در ارتباط با سرور' };
  return data;
}
