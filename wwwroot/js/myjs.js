'use strict';

// Reads the hidden anti-forgery token from the page for use in AJAX requests
export function getAntiForgeryToken() {
    return document.querySelector('input[name="__RequestVerificationToken"]')?.value ?? '';
}

// Sends a POST request with FormData and returns the parsed JSON response
export async function ajaxPost(url, formData) {
    const response = await fetch(url, {
        method: 'POST',
        headers: { 'X-Requested-With': 'XMLHttpRequest' },
        body: formData,
    });
    if (!response.ok) throw new Error(await response.text());
    return response.json();
}

// Delays invoking fn until the user stops triggering it for `wait` milliseconds
export function debounce(fn, wait) {
    let timer;
    return function (...args) {
        clearTimeout(timer);
        timer = setTimeout(() => fn.apply(this, args), wait);
    };
}

// Creates a temporary Bootstrap alert toast that auto-removes after 3 seconds
export function showToast(message, type) {
    type = type || 'success';

    let container = document.getElementById('toast-container');
    if (!container) {
        container = document.createElement('div');
        container.id = 'toast-container';
        container.style.cssText = 'position:fixed;top:1rem;right:1rem;z-index:9999;display:flex;flex-direction:column;gap:0.5rem;';
        document.body.appendChild(container);
    }

    const toast = document.createElement('div');
    toast.className = 'alert alert-' + type + ' shadow mb-0';
    toast.style.cssText = 'min-width:220px;opacity:1;transition:opacity 0.4s ease;';
    toast.textContent = message;
    container.appendChild(toast);

    setTimeout(function () {
        toast.style.opacity = '0';
        setTimeout(function () { toast.remove(); }, 400);
    }, 3000);
}
