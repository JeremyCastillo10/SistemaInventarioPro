window.localStorageFunctions = {
    setToken: (key, value) => {
        localStorage.setItem(key, value);
    },
    getToken: (key) => {
        return localStorage.getItem(key);
    },
    removeToken: (key) => {
        localStorage.removeItem(key);
    }
};
