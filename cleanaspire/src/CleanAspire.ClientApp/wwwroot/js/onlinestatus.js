
window.onlineStatusInterop = {
    getOnlineStatus: function () {
        return navigator.onLine;
    },
    addOnlineStatusListener: function (dotNetObjectRef) {
        const onlineHandler = () => {
            dotNetObjectRef.invokeMethodAsync('UpdateOnlineStatus', true);
        };
        const offlineHandler = () => {
            dotNetObjectRef.invokeMethodAsync('UpdateOnlineStatus', false);
        };

        window.addEventListener('online', onlineHandler);
        window.addEventListener('offline', offlineHandler);
    }
};