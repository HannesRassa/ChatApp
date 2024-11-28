import { defineStore } from 'pinia';

export const useUserStore = defineStore('user', {
  state: () => ({
    userId: null as number | null,
    username: '',
  }),
  actions: {
    setUser(id: number, username: string) {
      this.userId = id;
      this.username = username;

      // Save to localStorage
      localStorage.setItem('user', JSON.stringify({ id, username }));
    },
    logOut() {
      this.userId = null;
      this.username = '';

      // Remove from localStorage
      localStorage.removeItem('user');
    },
    loadUser() {
      const user = localStorage.getItem('user');
      if (user) {
        const { id, username } = JSON.parse(user);
        this.userId = id;
        this.username = username;
      }
    },
  },
});
