self.addEventListener('push', (event) => {
    const data = event.data ? event.data.json() : { title: 'Default Title', body: 'Default Body' };

    const options = {
        body: data.body,
        icon: 'img/icon.png',
        badge: 'img/badge.png',
    };

    event.waitUntil(
        self.registration.showNotification(data.title, options)
    );
});