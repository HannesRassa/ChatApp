<template>
  <div class="packs-container">
    <!-- Home Button -->
    <button class="home-button" @click="goHome">Home</button>

    <h1 class="page-title">Question Packages</h1>
    <ul class="pack-list">
      <li v-for="pack in packs" :key="pack" class="pack-item">
        <span>{{ pack }}</span>
        <button class="primary-button" @click="viewPack(pack)">View</button>
      </li>
    </ul>
  </div>
</template>

<script lang="ts">
import { defineComponent, ref, onMounted } from "vue";
import axios from "axios";
import { useRouter } from "vue-router";

export default defineComponent({
  name: "QuestionPacks",
  setup() {
    const packs = ref<string[]>([]);
    const router = useRouter();

    const fetchPacks = async () => {
      try {
        const response = await axios.get("https://localhost:7269/Backend/Question/Packs");
        packs.value = response.data;
      } catch (error) {
        console.error("Failed to fetch question packs:", error);
      }
    };

    const viewPack = (packName: string) => {
      router.push(`/question-packs/${encodeURIComponent(packName)}`);
    };

    const goHome = () => {
      router.push("/");
    };

    onMounted(() => {
      fetchPacks();
    });

    return {
      packs,
      viewPack,
      goHome,
    };
  },
});
</script>

<style scoped>
.packs-container {
  max-width: 600px;
  margin: 0 auto;
  padding: 20px;
  background-color: #000;
  border-radius: 8px;
  box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
}

.page-title {
  font-size: 24px;
  text-align: center;
  margin-bottom: 20px;
  color: #333;
}

.pack-list {
  list-style: none;
  padding: 0;
}

.pack-item {
  margin: 10px 0;
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.primary-button {
  background-color: #a607ce;
  color: white;
  padding: 8px 16px;
  border: none;
  border-radius: 5px;
  cursor: pointer;
}

.home-button {
  position: absolute;
  top: 10px;
  left: 10px;
  background-color: #9104a3;
  color: white;
  padding: 10px 20px;
  border: none;
  border-radius: 5px;
  cursor: pointer;
  font-size: 16px;
}

h1 {
  margin-bottom: 20px;
}
</style>
