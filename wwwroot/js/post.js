'use strict';

import { getAntiForgeryToken, showToast } from './myjs.js';

// Post Repository
// Handles all AJAX calls to the /api/posts JSON endpoint
export class PostRepository {
    #baseAddress;

    constructor(baseAddress) {
        this.#baseAddress = baseAddress;
    }

    // Sends a like or dislike reaction for a post
    async react(postId, type) {
        const fd = new FormData();
        fd.append('postId', postId);
        fd.append('type', type);
        fd.append('__RequestVerificationToken', getAntiForgeryToken());
        const response = await fetch(`${this.#baseAddress}/react`, {
            method: 'POST',
            headers: { 'X-Requested-With': 'XMLHttpRequest' },
            body: fd,
        });
        if (!response.ok) throw new Error(await response.text());
        return await response.json();
    }

    // Submits a new comment on a post and returns the saved comment data
    async addComment(postId, body) {
        const fd = new FormData();
        fd.append('postId', postId);
        fd.append('body', body);
        fd.append('__RequestVerificationToken', getAntiForgeryToken());
        const response = await fetch(`${this.#baseAddress}/add-comment`, {
            method: 'POST',
            headers: { 'X-Requested-With': 'XMLHttpRequest' },
            body: fd,
        });
        if (!response.ok) throw new Error(await response.text());
        return await response.json();
    }

    // Deletes a comment by its ID
    async deleteComment(id) {
        const token = getAntiForgeryToken();
        const response = await fetch(`${this.#baseAddress}/${id}`, {
            method: 'DELETE',
            headers: {
                'X-Requested-With': 'XMLHttpRequest',
                'RequestVerificationToken': token,
            },
        });
        if (!response.ok) throw new Error(await response.text());
        return await response.json();
    }

    // Updates the body text of an existing comment
    async editComment(id, body) {
        const fd = new FormData();
        fd.append('body', body);
        fd.append('__RequestVerificationToken', getAntiForgeryToken());
        const response = await fetch(`${this.#baseAddress}/${id}`, {
            method: 'PUT',
            headers: { 'X-Requested-With': 'XMLHttpRequest' },
            body: fd,
        });
        if (!response.ok) throw new Error(await response.text());
        return await response.json();
    }
}

const repo = new PostRepository('/api/PostsApi');

// Posts Index 
document.querySelectorAll('.btn-react').forEach(function (btn) {
    btn.addEventListener('click', async function () {
        const card = this.closest('[data-post]');
        try {
            const data = await repo.react(this.dataset.postId, this.dataset.type);
            card.querySelector('.like-count').textContent = data.likes;
            card.querySelector('.dislike-count').textContent = data.dislikes;
        } catch {
            showToast('Could not save reaction.', 'danger');
        }
    });
});

// Posts Details 
const likeBtn    = document.getElementById('btn-like');
const dislikeBtn = document.getElementById('btn-dislike');

// Updates the like/dislike counts on the details page after a reaction is saved
async function sendReaction(btn) {
    try {
        const data = await repo.react(btn.dataset.postId, btn.dataset.type);
        document.getElementById('like-count').textContent = data.likes;
        document.getElementById('dislike-count').textContent = data.dislikes;
    } catch {
        showToast('Could not save reaction. Try again.', 'danger');
    }
}

if (likeBtn) likeBtn.addEventListener('click', () => sendReaction(likeBtn));
if (dislikeBtn) dislikeBtn.addEventListener('click', () => sendReaction(dislikeBtn));

// Handles delete, edit, cancel, and save clicks on comment cards via event delegation
document.getElementById('comment-list')?.addEventListener('click', async function (e) {
    const card = e.target.closest('[data-comment-id]');
    if (!card) return;
    const commentId = card.dataset.commentId;
    const bodyEl    = card.querySelector('.comment-body');
    const editForm  = card.querySelector('.comment-edit-form');

    if (e.target.classList.contains('btn-delete-comment')) {
        if (!confirm('Delete this comment?')) return;
        try {
            await repo.deleteComment(commentId);
            card.remove();
            if (!document.querySelector('[data-comment-id]')) {
                const list = document.querySelector('.vstack.gap-3');
                if (list) {
                    list.insertAdjacentHTML('beforebegin', '<p id="no-comments" class="text-muted mb-0">No comments yet.</p>');
                    list.remove();
                }
            }
            showToast('Comment deleted.', 'success');
        } catch {
            showToast('Could not delete comment.', 'danger');
        }
    }

    if (e.target.classList.contains('btn-edit-comment')) {
        bodyEl.classList.add('d-none');
        editForm.classList.remove('d-none');
        editForm.querySelector('.edit-textarea').focus();
    }

    if (e.target.classList.contains('btn-cancel-edit')) {
        editForm.classList.add('d-none');
        bodyEl.classList.remove('d-none');
    }

    if (e.target.classList.contains('btn-save-comment')) {
        const newBody = editForm.querySelector('.edit-textarea').value;
        try {
            const data = await repo.editComment(commentId, newBody);
            bodyEl.textContent = data.body;
            editForm.querySelector('.edit-textarea').value = data.body;
            editForm.classList.add('d-none');
            bodyEl.classList.remove('d-none');
            showToast('Comment updated.', 'success');
        } catch {
            showToast('Could not save comment.', 'danger');
        }
    }
});

// Submits the new comment form via AJAX and prepends the new card to the list
const commentForm = document.getElementById('comment-form');
if (commentForm) {
    commentForm.addEventListener('submit', async function (e) {
        e.preventDefault();
        document.getElementById('comment-error').classList.add('d-none');

        const fd = new FormData(commentForm);
        try {
            const data = await repo.addComment(fd.get('postId'), fd.get('body'));

            document.getElementById('no-comments')?.remove();

            let list = commentForm.closest('.row').querySelector('.vstack');
            if (!list) {
                list = document.createElement('div');
                list.className = 'vstack gap-3';
                document.getElementById('comment-list').appendChild(list);
            }

            const card = document.createElement('div');
            card.className = 'border rounded-4 p-3';
            card.innerHTML = `<div class="small text-muted">${data.username} · ${data.createdAt}</div><div class="mt-2">${data.body}</div>`;
            list.prepend(card);

            document.getElementById('comment-body').value = '';
            showToast('Comment posted!', 'success');
        } catch {
            showToast('Failed to post comment. Please try again.', 'danger');
        }
    });
}

// Posts Create 
// Fetches the current user's teams and populates the team dropdown on create/edit forms
(async function () {
    const select = document.getElementById('team-select');
    if (!select) return;

    const preselectedId = select.dataset.preselected;
    try {
        const res = await fetch('/Teams/UserTeams', {
            headers: { 'X-Requested-With': 'XMLHttpRequest' }
        });
        const teams = await res.json();

        if (teams.length === 0) {
            select.innerHTML = '<option value="">you have no teams yet</option>';
            return;
        }

        select.innerHTML = '<option value="">-- select a team --</option>';
        teams.forEach(t => {
            const opt = document.createElement('option');
            opt.value = t.id;
            opt.textContent = t.name;
            if (t.id === preselectedId) opt.selected = true;
            select.appendChild(opt);
        });
    } catch {
        select.innerHTML = '<option value="">could not load teams</option>';
    }
})();
