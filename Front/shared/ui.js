export function toast(msg, type = '') {
  let el = document.getElementById('toast');
  if (!el) {
    el = document.createElement('div');
    el.id = 'toast';
    document.body.appendChild(el);
  }
  el.textContent = msg;
  el.className = type ? `show ${type}` : 'show';
  clearTimeout(el._t);
  el._t = setTimeout(() => { el.className = ''; }, 3000);
}

export function setLoading(btn, loading) {
  if (loading) {
    btn._text = btn.innerHTML;
    btn.innerHTML = '<span class="spinner"></span>';
    btn.disabled = true;
  } else {
    btn.innerHTML = btn._text ?? btn.innerHTML;
    btn.disabled = false;
  }
}

export function formatDate(iso) {
  if (!iso) return '—';
  const d = new Date(iso);
  return d.toLocaleDateString('fa-IR');
}

export function formatTime(t) {
  if (!t) return '—';
  return t.substring(0, 5);
}

export function statusBadge(status) {
  const map = {
    Pending:   ['در انتظار', 'badge-amber'],
    Confirmed: ['تأیید شده', 'badge-teal'],
    Cancelled: ['لغو شده',   'badge-coral'],
    Completed: ['انجام شده', 'badge-gray'],
  };
  const [label, cls] = map[status] ?? [status, 'badge-gray'];
  return `<span class="badge ${cls}">${label}</span>`;
}

export function empty(msg = 'موردی یافت نشد') {
  return `<div class="empty-state"><i class="ti ti-inbox"></i><p>${msg}</p></div>`;
}
