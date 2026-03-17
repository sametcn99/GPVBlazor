window.gpvAuth = {
    getCookieValue: function (name) {
        const encodedName = `${name}=`;
        const cookies = document.cookie ? document.cookie.split(";") : [];

        for (const cookie of cookies) {
            const trimmedCookie = cookie.trim();
            if (trimmedCookie.startsWith(encodedName)) {
                return decodeURIComponent(trimmedCookie.substring(encodedName.length));
            }
        }

        return "";
    },

    buildCsrfHeaders: function () {
        const token = window.gpvAuth.getCookieValue("gpv.csrf");
        return token ? { "X-CSRF-TOKEN": token } : {};
    },

    signInWithToken: async function (token) {
        const response = await fetch("/api/auth/personal-access-token", {
            method: "POST",
            credentials: "same-origin",
            headers: {
                "Content-Type": "application/json",
                ...window.gpvAuth.buildCsrfHeaders()
            },
            body: JSON.stringify({ token })
        });

        return response.ok;
    },

    signOut: async function () {
        const response = await fetch("/api/auth/logout", {
            method: "POST",
            credentials: "same-origin",
            headers: window.gpvAuth.buildCsrfHeaders()
        });

        return response.ok;
    }
};
