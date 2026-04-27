'use strict';

// Collection of global DOM behaviours initialised on every page load
export class AppDOM {
    // Fades out and removes success/danger alerts after 4 seconds
    static initAlertAutoDismiss() {
        document.querySelectorAll('.alert-success, .alert-danger').forEach(function (alert) {
            setTimeout(function () {
                alert.style.transition = 'opacity 0.6s ease';
                alert.style.opacity = '0';
                setTimeout(function () { alert.remove(); }, 600);
            }, 4000);
        });
    }

    // Shows a confirmation dialog before any form with data-confirm submits
    static initDeleteConfirmation() {
        document.querySelectorAll('form[data-confirm]').forEach(function (form) {
            form.addEventListener('submit', function (e) {
                const message = form.dataset.confirm || 'Are you sure? This cannot be undone.';
                if (!confirm(message)) {
                    e.preventDefault();
                }
            });
        });
    }

    // Adds a live character counter below any textarea with data-maxlength
    static initCharacterCounters() {
        document.querySelectorAll('textarea[data-maxlength]').forEach(function (el) {
            const max = parseInt(el.dataset.maxlength, 10);

            const counter = document.createElement('div');
            counter.className = 'form-text text-end';
            counter.textContent = '0 / ' + max;
            el.parentNode.insertBefore(counter, el.nextSibling);

            el.addEventListener('input', function () {
                const len = el.value.length;
                counter.textContent = len + ' / ' + max;
                counter.className = 'form-text text-end ' + (len > max * 0.9 ? 'text-warning fw-bold' : '');
            });
        });
    }

    // Highlights the nav link whose href matches the current URL path
    static initActiveNavLink() {
        const path = window.location.pathname.toLowerCase();
        document.querySelectorAll('.nav-link[href]').forEach(function (link) {
            const href = link.getAttribute('href').toLowerCase();
            if (href !== '/' && path.startsWith(href)) {
                link.classList.add('active');
            }
        });
    }

    // Displays a sprite preview image when a pokemon:selected custom event fires
    static initPokemonSpritePreview() {
        const input = document.getElementById('pokemon-input');
        if (!input) return;

        const wrapper = document.createElement('div');
        wrapper.id = 'sprite-preview';
        wrapper.className = 'text-center mt-2 d-none';

        const img = document.createElement('img');
        img.id = 'pokemon-sprite-img';
        img.alt = '';
        img.style.cssText = 'width:96px;height:96px;image-rendering:pixelated;';

        const label = document.createElement('div');
        label.id = 'pokemon-sprite-label';
        label.className = 'small text-muted mt-1';

        wrapper.appendChild(img);
        wrapper.appendChild(label);
        input.parentNode.appendChild(wrapper);

        document.addEventListener('pokemon:selected', function (e) {
            const { name, sprite } = e.detail;
            if (sprite) {
                img.src = sprite;
                label.textContent = name;
                wrapper.classList.remove('d-none');
            }
        });

        input.addEventListener('input', function () {
            if (!this.value.trim()) {
                wrapper.classList.add('d-none');
                img.src = '';
                label.textContent = '';
            }
        });
    }

    // Injects a copy-link button that copies the current page URL to the clipboard
    static initShareButton() {
        const target = document.getElementById('share-target');
        if (!target) return;

        const btn = document.createElement('button');
        btn.type = 'button';
        btn.className = 'btn btn-outline-secondary btn-sm';
        btn.textContent = 'copy link';
        target.appendChild(btn);

        btn.addEventListener('click', function () {
            navigator.clipboard.writeText(window.location.href).then(function () {
                btn.textContent = 'copied!';
                setTimeout(function () { btn.textContent = 'copy link'; }, 2000);
            });
        });
    }
}

// Run all DOM initialisers once the page has fully loaded
document.addEventListener('DOMContentLoaded', function () {
    AppDOM.initAlertAutoDismiss();
    AppDOM.initDeleteConfirmation();
    AppDOM.initCharacterCounters();
    AppDOM.initActiveNavLink();
    AppDOM.initPokemonSpritePreview();
    AppDOM.initShareButton();
});
