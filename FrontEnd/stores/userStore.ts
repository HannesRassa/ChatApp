// stores/userStore.ts
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
    },
    logOut() {
      this.userId = null;
      this.username = '';
    },
  },
});
