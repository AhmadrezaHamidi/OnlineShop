// ── Demo mode (مرورگر، بدون Electron) ────────────────────────────────────────
if (!window.api) {
  const MOCK = {
    orders:   { items: [{id:1001,userId:5,totalAmount:2500000,paymentMethod:0,status:2,createdAt:'2026-07-01'},{id:1002,userId:8,totalAmount:850000,paymentMethod:1,status:1,createdAt:'2026-07-01'},{id:1003,userId:3,totalAmount:4200000,paymentMethod:0,status:4,createdAt:'2026-06-30'}], totalCount:3 },
    products: { items: [
      {id:1, name:'کیف دوشی چرم',              price:2800000, stock:28,  status:1},
      {id:2, name:'کوله‌پشتی لپ‌تاپ ۱۵ اینچ', price:1450000, stock:35,  status:1},
      {id:3, name:'جاکلیدی فلزی آلومینیوم',    price:150000,  stock:120, status:1},
      {id:4, name:'تی‌شرت اسپرت مردانه',       price:320000,  stock:60,  status:1},
      {id:5, name:'شلوار جین زنانه استرچ',      price:950000,  stock:45,  status:1},
      {id:6, name:'هودی دخترانه زمستانی',       price:780000,  stock:18,  status:1},
      {id:7, name:'پیراهن رسمی مردانه',         price:1100000, stock:22,  status:1},
      {id:8, name:'کفش ورزشی نایک ایر مکس',    price:1800000, stock:15,  status:1},
      {id:9, name:'کفش کوهنوردی سالومون X',     price:3200000, stock:8,   status:1},
      {id:10,name:'کتانی اسپرت آدیداس اولترا',  price:2100000, stock:24,  status:1},
      {id:11,name:'هدفون بی‌سیم سونی WH-1000',  price:4500000, stock:12,  status:1},
      {id:12,name:'پاوربانک ۱۰۰۰۰ میلی‌آمپر',  price:680000,  stock:50,  status:1},
      {id:13,name:'کابل شارژ USB-C دو متری',    price:95000,   stock:200, status:1},
      {id:14,name:'دمبل ۵ کیلوگرمی لاستیکی',   price:540000,  stock:30,  status:1},
      {id:15,name:'یوگا مت حرفه‌ای ۶ میلی‌متر', price:890000,  stock:18,  status:1},
    ], totalCount:15 },
    users:    { items: [{id:5,fullName:'علی احمدی',phoneNumber:'09121234567',status:0},{id:8,fullName:'مریم رضایی',phoneNumber:'09351234567',status:0},{id:3,fullName:'حسن موسوی',phoneNumber:'09011234567',status:1}], totalCount:3 },
    bnpl:     { items: [{id:201,orderId:1002,totalAmount:2550000,installmentCount:6,status:0},{id:202,orderId:998,totalAmount:1200000,installmentCount:3,status:2}], totalCount:2 },
    admins:   { items: [{id:1,fullName:'احمد حمیدی‌فرد',email:'vestaabnergame@gmail.com',role:'SuperAdmin',status:0}], totalCount:1 },
  };
  window.api = {
    getConfig:  ()           => Promise.resolve({ apiUrl:'http://localhost:5000', token:'demo' }),
    saveConfig: (u)          => Promise.resolve({ apiUrl:'http://localhost:5000', token:'demo', ...u }),
    fetch: (path, _opts={}) => {
      if (path.includes('/Order'))    return Promise.resolve({ ok:true, status:200, data:{ result: MOCK.orders  } });
      if (path.includes('/Product'))  return Promise.resolve({ ok:true, status:200, data:{ result: MOCK.products} });
      if (path.includes('/Identity/Users')) return Promise.resolve({ ok:true, status:200, data:{ result: MOCK.users   } });
      if (path.includes('/Bnpl'))     return Promise.resolve({ ok:true, status:200, data:{ result: MOCK.bnpl    } });
      if (path.includes('/BackOffice')) return Promise.resolve({ ok:true, status:200, data:{ result: MOCK.admins  } });
      if (path.includes('/health'))   return Promise.resolve({ ok:true, status:200, data:'ok' });
      return Promise.resolve({ ok:true, status:200, data:{} });
    },
  };
}

// ── State ─────────────────────────────────────────────────────────────────────
let config  = { apiUrl: 'http://localhost:5000', token: null };
let currentPage = 'dashboard';

const API_VERSION = 'api/v1';

// ── Bootstrap ─────────────────────────────────────────────────────────────────
window.addEventListener('DOMContentLoaded', async () => {
  config = await window.api.getConfig();
  document.getElementById('api-url-input').value    = config.apiUrl;
  document.getElementById('settings-api-url').value = config.apiUrl;

  if (config.token) {
    showApp();
    loadCurrentPage();
    checkApiHealth();
  }

  bindEvents();
  setInterval(checkApiHealth, 30_000);
});

// ── Events ────────────────────────────────────────────────────────────────────
function bindEvents() {
  // Login
  document.getElementById('btn-send-otp').addEventListener('click', sendOtp);
  document.getElementById('btn-verify-otp').addEventListener('click', verifyOtp);
  document.getElementById('btn-back-phone').addEventListener('click', () => {
    show('step-phone'); hide('step-otp');
    document.getElementById('otp-error').textContent = '';
  });
  document.getElementById('phone-input').addEventListener('keydown', e => { if (e.key === 'Enter') sendOtp(); });
  document.getElementById('otp-input').addEventListener('keydown',   e => { if (e.key === 'Enter') verifyOtp(); });

  // API URL in login box — save on change
  document.getElementById('api-url-input').addEventListener('change', async e => {
    config = await window.api.saveConfig({ apiUrl: e.target.value.trim() });
  });

  // Nav
  document.querySelectorAll('.nav-item[data-page]').forEach(el => {
    el.addEventListener('click', () => navigateTo(el.dataset.page));
  });

  // Topbar
  document.getElementById('btn-refresh').addEventListener('click', loadCurrentPage);
  document.getElementById('btn-logout').addEventListener('click', logout);

  // Orders filter
  document.getElementById('btn-filter-orders').addEventListener('click', () => loadOrders(1));

  // Settings save
  document.getElementById('btn-save-settings').addEventListener('click', async () => {
    const url = document.getElementById('settings-api-url').value.trim();
    config = await window.api.saveConfig({ apiUrl: url });
    document.getElementById('api-url-input').value = url;
    toast('تنظیمات ذخیره شد', 'success');
    checkApiHealth();
  });
}

// ── Auth ──────────────────────────────────────────────────────────────────────
async function sendOtp() {
  const phone = document.getElementById('phone-input').value.trim();
  document.getElementById('phone-error').textContent = '';

  if (!/^09[0-9]{9}$/.test(phone)) {
    document.getElementById('phone-error').textContent = 'شماره موبایل معتبر نیست';
    return;
  }

  setLoading('btn-send-otp', true);
  const res = await window.api.fetch(`/${API_VERSION}/Identity/Auth/RequestOtp`, {
    method: 'POST',
    body:   { phoneNumber: phone },
    auth:   false,
  });
  setLoading('btn-send-otp', false);

  if (res.ok) {
    document.getElementById('otp-phone-label').textContent = phone;
    hide('step-phone');
    show('step-otp');
    document.getElementById('otp-input').focus();
  } else {
    document.getElementById('phone-error').textContent = extractError(res) || 'خطا در ارسال کد';
  }
}

async function verifyOtp() {
  const phone = document.getElementById('phone-input').value.trim();
  const code  = document.getElementById('otp-input').value.trim();
  document.getElementById('otp-error').textContent = '';

  if (code.length !== 6) {
    document.getElementById('otp-error').textContent = 'کد ۶ رقمی است';
    return;
  }

  setLoading('btn-verify-otp', true);
  const res = await window.api.fetch(`/${API_VERSION}/Identity/Auth/VerifyOtp`, {
    method: 'POST',
    body:   { phoneNumber: phone, code },
    auth:   false,
  });
  setLoading('btn-verify-otp', false);

  if (res.ok && res.data?.accessToken) {
    config = await window.api.saveConfig({ token: res.data.accessToken });
    showApp();
    loadCurrentPage();
    checkApiHealth();
  } else {
    document.getElementById('otp-error').textContent = extractError(res) || 'کد اشتباه یا منقضی شده';
  }
}

async function logout() {
  await window.api.fetch(`/${API_VERSION}/Identity/Auth/Logout`, { method: 'POST' });
  config = await window.api.saveConfig({ token: null });
  hide('app');
  show('login-overlay');
  hide('step-otp');
  show('step-phone');
  document.getElementById('phone-input').value = '';
  document.getElementById('otp-input').value   = '';
}

// ── Navigation ────────────────────────────────────────────────────────────────
function navigateTo(page) {
  currentPage = page;

  document.querySelectorAll('.nav-item').forEach(el => {
    el.classList.toggle('active', el.dataset.page === page);
  });
  document.querySelectorAll('.page').forEach(el => {
    el.classList.toggle('active', el.id === `page-${page}`);
  });

  const titles = {
    dashboard: 'داشبورد', orders: 'سفارش‌ها', products: 'محصولات',
    users: 'کاربران', bnpl: 'اقساط BNPL', admins: 'مدیران سیستم', settings: 'تنظیمات',
  };
  document.getElementById('page-title').textContent = titles[page] || page;

  loadCurrentPage();
}

function loadCurrentPage() {
  switch (currentPage) {
    case 'dashboard': loadDashboard();  break;
    case 'orders':    loadOrders(1);    break;
    case 'products':  loadProducts(1);  break;
    case 'users':     loadUsers(1);     break;
    case 'bnpl':      loadBnpl();       break;
    case 'admins':    loadAdmins();     break;
  }
}

// ── Dashboard ─────────────────────────────────────────────────────────────────
async function loadDashboard() {
  setStatLoading();

  const [ordersRes, productsRes, bnplRes] = await Promise.all([
    window.api.fetch(`/${API_VERSION}/Order?pageSize=5`),
    window.api.fetch(`/${API_VERSION}/Product?pageSize=1&status=1`),
    window.api.fetch(`/${API_VERSION}/Bnpl?pageSize=50`),
  ]);

  if (ordersRes.ok) {
    const data = ordersRes.data?.result ?? ordersRes.data;
    const items = data?.items ?? [];
    const total = data?.totalCount ?? items.length;

    document.getElementById('stat-orders-today').textContent = total;
    renderRecentOrders(items.slice(0, 5));
  }

  if (productsRes.ok) {
    const total = productsRes.data?.result?.totalCount ?? productsRes.data?.totalCount ?? '—';
    document.getElementById('stat-products').textContent = total;
  }

  if (bnplRes.ok) {
    const items   = bnplRes.data?.result?.items ?? bnplRes.data?.items ?? [];
    const overdue = items.filter(c => c.status === 'Defaulted' || c.status === 3).length;
    document.getElementById('stat-overdue').textContent = overdue;
  }

  document.getElementById('stat-revenue').textContent = '—';
}

function setStatLoading() {
  ['stat-orders-today','stat-revenue','stat-products','stat-overdue'].forEach(id => {
    document.getElementById(id).innerHTML = '<div class="loader"></div>';
  });
}

function renderRecentOrders(items) {
  const tbody = document.getElementById('recent-orders-body');
  if (!items.length) { tbody.innerHTML = '<tr><td colspan="5" style="text-align:center;color:var(--text2)">داده‌ای یافت نشد</td></tr>'; return; }

  tbody.innerHTML = items.map(o => `
    <tr>
      <td>#${o.id}</td>
      <td>${o.userId ?? '—'}</td>
      <td>${fmtNum(o.totalAmount)}</td>
      <td>${orderStatusBadge(o.status)}</td>
      <td>${fmtDate(o.createdAt)}</td>
    </tr>`).join('');
}

// ── Orders ────────────────────────────────────────────────────────────────────
let orderPage = 1;
async function loadOrders(page = 1) {
  orderPage = page;
  const status = document.getElementById('order-status-filter').value;
  const qs     = new URLSearchParams({ page, pageSize: 20 });
  if (status !== '') qs.set('status', status);

  document.getElementById('orders-body').innerHTML =
    '<tr class="loading-row"><td colspan="6"><div class="loader"></div></td></tr>';

  const res = await window.api.fetch(`/${API_VERSION}/Order?${qs}`);
  if (!res.ok) { showTableError('orders-body', 6); return; }

  const data  = res.data?.result ?? res.data ?? {};
  const items = data.items ?? [];
  const total = data.totalCount ?? 0;

  document.getElementById('orders-body').innerHTML = items.length
    ? items.map(o => `
        <tr>
          <td>#${o.id}</td>
          <td>${o.userId ?? '—'}</td>
          <td>${fmtNum(o.totalAmount)}</td>
          <td>${paymentMethodLabel(o.paymentMethod)}</td>
          <td>${orderStatusBadge(o.status)}</td>
          <td>${fmtDate(o.createdAt)}</td>
        </tr>`).join('')
    : '<tr><td colspan="6" style="text-align:center;color:var(--text2)">سفارشی یافت نشد</td></tr>';

  renderPagination('orders-pagination', page, Math.ceil(total / 20), loadOrders);
}

// ── Products ──────────────────────────────────────────────────────────────────
async function loadProducts(page = 1) {
  document.getElementById('products-body').innerHTML =
    '<tr class="loading-row"><td colspan="5"><div class="loader"></div></td></tr>';

  const res = await window.api.fetch(`/${API_VERSION}/Product?page=${page}&pageSize=20`);
  if (!res.ok) { showTableError('products-body', 5); return; }

  const data  = res.data?.result ?? res.data ?? {};
  const items = data.items ?? [];
  const total = data.totalCount ?? 0;

  document.getElementById('products-body').innerHTML = items.length
    ? items.map(p => `
        <tr>
          <td>#${p.id}</td>
          <td>${p.name ?? '—'}</td>
          <td>${fmtNum(p.price)}</td>
          <td>${p.stock ?? p.quantity ?? '—'}</td>
          <td>${productStatusBadge(p.status)}</td>
        </tr>`).join('')
    : '<tr><td colspan="5" style="text-align:center;color:var(--text2)">محصولی یافت نشد</td></tr>';

  renderPagination('products-pagination', page, Math.ceil(total / 20), loadProducts);
}

// ── Users ─────────────────────────────────────────────────────────────────────
async function loadUsers(page = 1) {
  document.getElementById('users-body').innerHTML =
    '<tr class="loading-row"><td colspan="5"><div class="loader"></div></td></tr>';

  const res = await window.api.fetch(`/${API_VERSION}/Identity/Users?page=${page}&pageSize=20`);
  if (!res.ok) { showTableError('users-body', 5); return; }

  const data  = res.data?.result ?? res.data ?? {};
  const items = data.items ?? [];
  const total = data.totalCount ?? 0;

  document.getElementById('users-body').innerHTML = items.length
    ? items.map(u => `
        <tr>
          <td>#${u.id}</td>
          <td>${u.fullName ?? u.displayName ?? '—'}</td>
          <td dir="ltr">${u.phoneNumber ?? '—'}</td>
          <td>${userStatusBadge(u.status)}</td>
          <td>
            <button class="btn btn-sm" style="background:var(--bg3)"
              onclick="activateUser(${u.id})">فعال‌سازی</button>
          </td>
        </tr>`).join('')
    : '<tr><td colspan="5" style="text-align:center;color:var(--text2)">کاربری یافت نشد</td></tr>';

  renderPagination('users-pagination', page, Math.ceil(total / 20), loadUsers);
}

async function activateUser(id) {
  const res = await window.api.fetch(`/${API_VERSION}/Identity/Users/${id}/Activate`, { method: 'PATCH' });
  if (res.ok) { toast('کاربر فعال شد', 'success'); loadUsers(1); }
  else         toast(extractError(res) || 'خطا', 'error');
}

// ── BNPL ──────────────────────────────────────────────────────────────────────
async function loadBnpl() {
  document.getElementById('bnpl-body').innerHTML =
    '<tr class="loading-row"><td colspan="5"><div class="loader"></div></td></tr>';

  const res = await window.api.fetch(`/${API_VERSION}/Bnpl?pageSize=50`);
  if (!res.ok) { showTableError('bnpl-body', 5); return; }

  const items = res.data?.result?.items ?? res.data?.items ?? [];

  document.getElementById('bnpl-body').innerHTML = items.length
    ? items.map(c => `
        <tr>
          <td>#${c.id}</td>
          <td>#${c.orderId}</td>
          <td>${fmtNum(c.totalAmount)}</td>
          <td>${c.installmentCount}</td>
          <td>${bnplStatusBadge(c.status)}</td>
        </tr>`).join('')
    : '<tr><td colspan="5" style="text-align:center;color:var(--text2)">قراردادی یافت نشد</td></tr>';
}

// ── Admins ────────────────────────────────────────────────────────────────────
async function loadAdmins() {
  document.getElementById('admins-body').innerHTML =
    '<tr class="loading-row"><td colspan="5"><div class="loader"></div></td></tr>';

  const res = await window.api.fetch(`/${API_VERSION}/BackOffice/Admins?pageSize=50`);
  if (!res.ok) { showTableError('admins-body', 5); return; }

  const data  = res.data?.result ?? res.data ?? {};
  const items = data.items ?? [];

  document.getElementById('admins-body').innerHTML = items.length
    ? items.map(a => `
        <tr>
          <td>#${a.id}</td>
          <td>${a.fullName ?? '—'}</td>
          <td dir="ltr">${a.email ?? '—'}</td>
          <td>${a.role ?? '—'}</td>
          <td>${adminStatusBadge(a.status)}</td>
        </tr>`).join('')
    : '<tr><td colspan="5" style="text-align:center;color:var(--text2)">مدیری یافت نشد</td></tr>';
}

// ── Health Check ──────────────────────────────────────────────────────────────
async function checkApiHealth() {
  const el = document.getElementById('api-status');
  try {
    const res = await window.api.fetch('/health', { auth: false });
    const ok  = res.ok || res.status === 404; // 404 = API is up, just no /health endpoint
    el.textContent = ok ? '● آنلاین' : '● آفلاین';
    el.className   = ok ? 'online'  : 'offline';
  } catch {
    el.textContent = '● آفلاین';
    el.className   = 'offline';
  }
}

// ── UI Helpers ────────────────────────────────────────────────────────────────
function showApp() {
  hide('login-overlay');
  document.getElementById('app').style.display = 'flex';
}

function show(id) { document.getElementById(id).style.display = ''; }
function hide(id) { document.getElementById(id).style.display = 'none'; }

function setLoading(btnId, loading) {
  const btn = document.getElementById(btnId);
  btn.disabled = loading;
  btn.innerHTML = loading ? '<div class="loader" style="width:14px;height:14px;border-width:2px"></div>' : btn.dataset.label ?? btn.textContent;
  if (!btn.dataset.label && !loading) btn.dataset.label = btn.textContent;
}

function showTableError(tbodyId, cols) {
  document.getElementById(tbodyId).innerHTML =
    `<tr><td colspan="${cols}" style="text-align:center;color:var(--red)">خطا در بارگذاری داده‌ها</td></tr>`;
}

function renderPagination(containerId, current, total, loadFn) {
  const el = document.getElementById(containerId);
  if (total <= 1) { el.innerHTML = ''; return; }

  const pages = [];
  for (let i = 1; i <= Math.min(total, 10); i++) {
    pages.push(`<button class="btn btn-sm ${i === current ? 'btn-primary' : ''}"
      style="${i !== current ? 'background:var(--bg3)' : ''}"
      onclick="${loadFn.name}(${i})">${i}</button>`);
  }
  el.innerHTML = pages.join('');
}

function toast(msg, type = 'success') {
  const el = document.getElementById('toast');
  el.textContent = msg;
  el.className   = `show ${type}`;
  setTimeout(() => { el.className = type; }, 3000);
}

// ── Formatters ────────────────────────────────────────────────────────────────
function fmtNum(n) {
  if (n == null) return '—';
  return Number(n).toLocaleString('fa-IR');
}

function fmtDate(d) {
  if (!d) return '—';
  return new Date(d).toLocaleDateString('fa-IR');
}

function extractError(res) {
  if (!res?.data) return null;
  return res.data?.title ?? res.data?.message ?? res.data?.error ?? JSON.stringify(res.data).slice(0, 80);
}

// ── Status Badges ─────────────────────────────────────────────────────────────
function orderStatusBadge(s) {
  const map = {
    0: ['badge-gray',   'Pending'],
    1: ['badge-blue',   'Placed'],
    2: ['badge-yellow', 'Confirmed'],
    3: ['badge-blue',   'Shipped'],
    4: ['badge-green',  'Delivered'],
    5: ['badge-red',    'Cancelled'],
  };
  const [cls, label] = map[s] ?? ['badge-gray', s ?? '—'];
  return `<span class="badge ${cls}">${label}</span>`;
}

function productStatusBadge(s) {
  const map = {
    0: ['badge-gray',   'Draft'],
    1: ['badge-green',  'Active'],
    2: ['badge-yellow', 'Inactive'],
    3: ['badge-red',    'Archived'],
  };
  const [cls, label] = map[s] ?? ['badge-gray', s ?? '—'];
  return `<span class="badge ${cls}">${label}</span>`;
}

function userStatusBadge(s) {
  const map = {
    0: ['badge-green',  'Active'],
    1: ['badge-yellow', 'Inactive'],
    2: ['badge-red',    'Suspended'],
  };
  const [cls, label] = map[s] ?? ['badge-gray', s ?? '—'];
  return `<span class="badge ${cls}">${label}</span>`;
}

function adminStatusBadge(s) {
  const map = {
    0: ['badge-green',  'Active'],
    1: ['badge-yellow', 'Inactive'],
    2: ['badge-red',    'Suspended'],
  };
  const [cls, label] = map[s] ?? ['badge-gray', s ?? '—'];
  return `<span class="badge ${cls}">${label}</span>`;
}

function bnplStatusBadge(s) {
  const map = {
    0: ['badge-blue',   'Active'],
    1: ['badge-green',  'Completed'],
    2: ['badge-red',    'Defaulted'],
    3: ['badge-red',    'Defaulted'],
  };
  const [cls, label] = map[s] ?? ['badge-gray', s ?? '—'];
  return `<span class="badge ${cls}">${label}</span>`;
}

function paymentMethodLabel(m) {
  return { 0: 'آنلاین', 1: 'اقساط (BNPL)', 2: 'نقدی' }[m] ?? '—';
}
