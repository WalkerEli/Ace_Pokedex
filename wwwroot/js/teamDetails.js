'use strict';

// Drives the team details page: loads PokeAPI data and handles add/edit member forms
(async function () {
    const pokemonInput    = document.getElementById('pokemon-input');
    const pokemonDatalist = document.getElementById('pokemon-datalist');
    const abilitySelect   = document.getElementById('ability-select');
    const heldItemInput   = document.getElementById('held-item-input');
    const heldItemDL      = document.getElementById('held-item-datalist');
    const moveInputs      = [1, 2, 3, 4].map(n => document.getElementById(`move-input-${n}`));
    const moveDLs         = [1, 2, 3, 4].map(n => document.getElementById(`move-datalist-${n}`));

    // Converts an API slug like "fire-spin" to a display name "Fire Spin"
    function toDisplay(apiName) {
        return apiName.replace(/-/g, ' ').replace(/\b\w/g, c => c.toUpperCase());
    }

    // Replaces all options in a datalist with a new array of name strings
    function addOptions(datalist, names) {
        datalist.innerHTML = '';
        names.forEach(function (name) {
            const opt = document.createElement('option');
            opt.value = name;
            datalist.appendChild(opt);
        });
    }

    // Clears and disables the move/ability fields when no valid Pokemon is selected
    function resetAddForm() {
        moveInputs.forEach(function (input) {
            input.value = '';
            input.disabled = true;
            input.placeholder = 'select a pokemon first';
        });
        moveDLs.forEach(function (dl) { dl.innerHTML = ''; });
        if (abilitySelect) abilitySelect.innerHTML = '<option value="">-- select a pokemon first --</option>';
    }

    // ── Load Pokemon list and items list ────────────────────────────────────
    let allApiNames = [];
    const [pokemonRes, itemsRes] = await Promise.allSettled([
        fetch('https://pokeapi.co/api/v2/pokemon?limit=1302'),
        fetch('https://pokeapi.co/api/v2/item?limit=2500')
    ]);

    if (pokemonRes.status === 'fulfilled' && pokemonRes.value.ok) {
        const data = await pokemonRes.value.json();
        allApiNames = data.results.map(p => p.name);
        if (pokemonDatalist) addOptions(pokemonDatalist, allApiNames.map(toDisplay));
        if (pokemonInput) pokemonInput.placeholder = 'type to search pokemon...';
    } else {
        if (pokemonInput) { pokemonInput.placeholder = 'could not load pokemon'; pokemonInput.disabled = true; }
    }

    if (itemsRes.status === 'fulfilled' && itemsRes.value.ok) {
        const data = await itemsRes.value.json();
        if (heldItemDL) addOptions(heldItemDL, data.results.map(i => toDisplay(i.name)));
        if (heldItemInput) heldItemInput.placeholder = 'type to search items...';
    } else {
        if (heldItemInput) { heldItemInput.placeholder = 'could not load items'; heldItemInput.disabled = true; }
    }

    // ── Add member: Pokemon change ───────────────────────────────────────────
    if (pokemonInput) {
        pokemonInput.addEventListener('change', async function () {
            const normalized = this.value.trim().toLowerCase().replace(/\s+/g, '-');

            if (!normalized || !allApiNames.includes(normalized)) {
                resetAddForm();
                return;
            }

            try {
                const res = await fetch(`https://pokeapi.co/api/v2/pokemon/${normalized}`);
                if (!res.ok) { resetAddForm(); return; }
                const pokemon = await res.json();

                abilitySelect.innerHTML = '<option value="">-- select ability --</option>';
                pokemon.abilities.forEach(function (a) {
                    const opt = document.createElement('option');
                    opt.value = toDisplay(a.ability.name);
                    opt.textContent = toDisplay(a.ability.name);
                    abilitySelect.appendChild(opt);
                });

                document.dispatchEvent(new CustomEvent('pokemon:selected', {
                    detail: { name: toDisplay(pokemon.name), sprite: pokemon.sprites.front_default }
                }));

                const moveNames = pokemon.moves.map(m => toDisplay(m.move.name));
                moveDLs.forEach(function (dl) { addOptions(dl, moveNames); });
                moveInputs.forEach(function (input) {
                    input.disabled = false;
                    input.placeholder = 'type to search moves...';
                });
            } catch {
                // silently ignore network errors
            }
        });
    }

    // ── Edit member helpers ──────────────────────────────────────────────────
    // Fetches Pokemon data from PokeAPI and populates the edit panel's ability/move fields
    async function loadEditPanel(panel, apiName, currentAbility) {
        const editAbilitySelect = panel.querySelector('.edit-ability-select');
        const editMoveInputs    = panel.querySelectorAll('.edit-move-input');
        const editMoveDL        = panel.querySelector('datalist[id^="edit-moves-"]');

        if (!apiName) return;

        try {
            const res = await fetch(`https://pokeapi.co/api/v2/pokemon/${apiName}`);
            if (!res.ok) return;
            const pokemon = await res.json();

            editAbilitySelect.innerHTML = '<option value="">-- select ability --</option>';
            pokemon.abilities.forEach(function (a) {
                const opt = document.createElement('option');
                const display = toDisplay(a.ability.name);
                opt.value = display;
                opt.textContent = display;
                if (display.toLowerCase() === currentAbility.toLowerCase()) opt.selected = true;
                editAbilitySelect.appendChild(opt);
            });

            addOptions(editMoveDL, pokemon.moves.map(m => toDisplay(m.move.name)));
            editMoveInputs.forEach(function (input) { input.disabled = false; });
        } catch {
            // silently ignore network errors
        }
    }

    // ── Edit member: open / close ────────────────────────────────────────────
    document.querySelectorAll('.btn-edit-member').forEach(function (btn) {
        btn.addEventListener('click', async function () {
            const panel = document.getElementById(`edit-panel-${this.dataset.memberId}`);
            const isOpen = !panel.classList.contains('d-none');

            document.querySelectorAll('.edit-member-panel').forEach(function (p) {
                p.classList.add('d-none');
            });

            if (isOpen) return;

            panel.classList.remove('d-none');
            await loadEditPanel(panel, this.dataset.apiName, this.dataset.ability || '');
        });
    });

    document.querySelectorAll('.btn-cancel-edit-member').forEach(function (btn) {
        btn.addEventListener('click', function () {
            this.closest('.edit-member-panel').classList.add('d-none');
        });
    });

    // ── Edit member: Pokemon change re-loads abilities and moves ─────────────
    document.querySelectorAll('.edit-pokemon-input').forEach(function (input) {
        input.addEventListener('change', async function () {
            const panel           = this.closest('.edit-member-panel');
            const editAbilitySelect = panel.querySelector('.edit-ability-select');
            const editMoveInputs  = panel.querySelectorAll('.edit-move-input');
            const editMoveDL      = panel.querySelector('datalist[id^="edit-moves-"]');

            const normalized = this.value.trim().toLowerCase().replace(/\s+/g, '-');

            if (!normalized || !allApiNames.includes(normalized)) {
                editAbilitySelect.innerHTML = '<option value="">-- select a pokemon first --</option>';
                editMoveInputs.forEach(function (inp) { inp.disabled = true; inp.value = ''; });
                editMoveDL.innerHTML = '';
                return;
            }

            try {
                const res = await fetch(`https://pokeapi.co/api/v2/pokemon/${normalized}`);
                if (!res.ok) return;
                const pokemon = await res.json();

                editAbilitySelect.innerHTML = '<option value="">-- select ability --</option>';
                pokemon.abilities.forEach(function (a) {
                    const opt = document.createElement('option');
                    opt.value = toDisplay(a.ability.name);
                    opt.textContent = toDisplay(a.ability.name);
                    editAbilitySelect.appendChild(opt);
                });

                addOptions(editMoveDL, pokemon.moves.map(m => toDisplay(m.move.name)));
                editMoveInputs.forEach(function (inp) {
                    inp.disabled = false;
                    inp.value = '';
                    inp.placeholder = 'type to search moves...';
                });
            } catch {
                // silently ignore network errors
            }
        });
    });

})();
