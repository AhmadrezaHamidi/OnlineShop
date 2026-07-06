const { app, BrowserWindow, ipcMain, shell } = require('electron');
const path = require('path');
const fs   = require('fs');

const CONFIG_FILE = path.join(app.getPath('userData'), 'config.json');

function loadConfig() {
  try {
    if (fs.existsSync(CONFIG_FILE))
      return JSON.parse(fs.readFileSync(CONFIG_FILE, 'utf8'));
  } catch {}
  return { apiUrl: 'http://localhost:5141', token: null };
}

function saveConfig(cfg) {
  fs.writeFileSync(CONFIG_FILE, JSON.stringify(cfg, null, 2));
}

let mainWindow;
let config = loadConfig();

function createWindow() {
  mainWindow = new BrowserWindow({
    width:  1280,
    height: 800,
    minWidth:  960,
    minHeight: 600,
    title: 'پنل مدیریت فروشگاه احمد',
    webPreferences: {
      preload:        path.join(__dirname, 'preload.js'),
      contextIsolation: true,
      nodeIntegration:  false,
    },
    icon: path.join(__dirname, 'assets', 'icon.png'),
    show: false,
  });

  mainWindow.loadFile('index.html');

  mainWindow.once('ready-to-show', () => mainWindow.show());

  mainWindow.webContents.setWindowOpenHandler(({ url }) => {
    shell.openExternal(url);
    return { action: 'deny' };
  });
}

app.whenReady().then(() => {
  createWindow();
  app.on('activate', () => {
    if (BrowserWindow.getAllWindows().length === 0) createWindow();
  });
});

app.on('window-all-closed', () => {
  if (process.platform !== 'darwin') app.quit();
});

// ── IPC handlers ─────────────────────────────────────────────────────────────

ipcMain.handle('get-config', () => config);

ipcMain.handle('save-config', (_, updates) => {
  config = { ...config, ...updates };
  saveConfig(config);
  return config;
});

ipcMain.handle('api-fetch', async (_, { path: apiPath, method = 'GET', body, auth = true }) => {
  const headers = { 'Content-Type': 'application/json' };
  if (auth && config.token) headers['Authorization'] = `Bearer ${config.token}`;

  const opts = { method, headers };
  if (body) opts.body = JSON.stringify(body);

  const url = `${config.apiUrl}${apiPath}`;
  const res  = await fetch(url, opts);
  const text = await res.text();
  let data;
  try { data = JSON.parse(text); } catch { data = text; }
  return { ok: res.ok, status: res.status, data };
});
