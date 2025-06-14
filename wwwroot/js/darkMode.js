window.darkMode = {
    set: function (enabled) {
        if (enabled) {
            document.body.classList.add('dark-mode');
        } else {
            document.body.classList.remove('dark-mode');
        }
        localStorage.setItem('dark_mode', enabled ? '1' : '0');
    },
    get: function () {
        return localStorage.getItem('dark_mode') === '0' ? false : true;
    },
    applyFromStorage: function () {
        var enabled = window.darkMode.get();
        window.darkMode.set(enabled);
    }
};

// Apply immediately to prevent FOUC
window.darkMode && window.darkMode.applyFromStorage && window.darkMode.applyFromStorage();