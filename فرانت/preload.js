const { contextBridge, ipcRenderer } = require('electron');

contextBridge.exposeInMainWorld('api', {
  getConfig:  ()                 => ipcRenderer.invoke('get-config'),
  saveConfig: (updates)          => ipcRenderer.invoke('save-config', updates),
  fetch:      (path, opts = {})  => ipcRenderer.invoke('api-fetch', { path, ...opts }),
});
