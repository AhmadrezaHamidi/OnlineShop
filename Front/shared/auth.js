import { getToken, getUser, logout } from './api.js';

export function requireAuth(allowedRoles) {
  const token = getToken();
  const user  = getUser();

  if (!token || !user) {
    window.location.href = getLoginPage(allowedRoles);
    return false;
  }

  if (allowedRoles && !allowedRoles.includes(user.role)) {
    alert('دسترسی ندارید');
    window.location.href = getLoginPage(allowedRoles);
    return false;
  }

  return true;
}

function getLoginPage(roles) {
  if (!roles) return '../shared/login.html';
  if (roles.includes('Admin'))  return '../admin/index.html';
  if (roles.includes('Doctor')) return '../doctor/index.html';
  return '../patient/index.html';
}

export function setupLogout(btnId) {
  const btn = document.getElementById(btnId);
  if (!btn) return;
  btn.addEventListener('click', () => {
    logout();
    window.location.href = btn.dataset.redirect || '../patient/index.html';
  });
}

export function fillUserInfo(nameElId) {
  const user = getUser();
  if (!user) return;
  const el = document.getElementById(nameElId);
  if (el) el.textContent = `${user.firstName} ${user.lastName}`;
}
