<template>
  <div>
    <button @click="startGame">Start</button>
  </div>
</template>

<script>
export default {
  methods: {
    startGame() {
      console.log("Game Started!");
    }
  }
};
</script>

<style scoped>
button {
  padding: 10px 20px;
  font-size: 16px;
  cursor: pointer;
}
</style>
