import { defineStore } from 'pinia';

export const useUserStore = defineStore('user', {
  state: () => ({
    userId: null as number | null,
    username: '',
    loggedIn: false,
  }),
  actions: {
    setUser(id: number, username: string) {
      this.userId = id;
      this.username = username;
      this.loggedIn = true;
    },
    logOut() {
      this.userId = null;
      this.username = '';
      this.loggedIn = false;
    },
  },
});
