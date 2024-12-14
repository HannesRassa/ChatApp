import { defineStore } from 'pinia';

export const useUserStore = defineStore('user', {
  state: () => ({
    userId: null as number | null,
    username: '',
    token: '',
    tokenExpiration: '',

  }),
  actions: {
    setUser(id: number, username: string, token: string, tokenExpiration: string) {
      this.userId = id;
      this.username = username;
      this.token = token;
      this.tokenExpiration = tokenExpiration;
      localStorage.setItem('user', JSON.stringify({ id, username, token, tokenExpiration }));
    },

    logOut() {
      this.userId = null;
      this.username = '';
      this.token = '';
      this.tokenExpiration = '';

      localStorage.removeItem('user');
    },
    loadUser() {
      const user = localStorage.getItem('user');
      if (user) {
        const { id, username, token, tokenExpiration } = JSON.parse(user);
        this.userId = id;
        this.username = username;
        this.token = token;
        this.tokenExpiration = tokenExpiration;

      }
    },
  },
});
