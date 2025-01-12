<template>
  <div>
    <el-button type="primary" @click="openAddAccount"> 新增 </el-button>
    <el-table :data="state.data" style="width: 100%" v-loading="state.loading" border>
      <el-table-column type="index" label="序号" width="55" align="center" fixed />
      <el-table-column prop="acmeId" label="acme" align="center" show-overflow-tooltip />
      <el-table-column prop="email" label="邮箱" align="center" show-overflow-tooltip />
      <el-table-column prop="name" label="描述" align="center" show-overflow-tooltip />
      <el-table-column label="操作" width="240" fixed="right" align="center" show-overflow-tooltip>
        <template #default="scope">
          <el-button size="small" text type="primary" @click="openEditAccount(scope.row)"> 编辑 </el-button>
          <el-button size="small" text type="danger" @click="delAccount(scope.row)"> 删除 </el-button>
        </template>
      </el-table-column>

    </el-table>
    <el-pagination v-model:currentPage="state.tableParams.page"
                   v-model:page-size="state.tableParams.pageSize"
                   :total="state.tableParams.total"
                   :page-sizes="[10, 20, 50, 100]"
                   size="small"
                   background
                   @size-change="handleSizeChange"
                   @current-change="handleCurrentChange"
                   layout="total, sizes, prev, pager, next, jumper" />
    <accountEdit ref="editAccountRef" @handleQuery="handleQuery" />
  </div>
</template>

<script setup lang="ts">
  import { ref, reactive, onMounted } from 'vue'
  import { ElMessageBox, ElMessage } from 'element-plus';
  import client, { CertificateAuthority, AcmeInfoUpdateInput } from '@/api';
  import accountEdit from './components/account-edit.vue'
  const editAccountRef = ref<InstanceType<typeof accountEdit>>();

  const state = reactive({
    loading: false,
    data: [] as Array<SysRole>,
    //queryParams: {
    //  name: undefined,
    //  code: undefined,
    //},
    tableParams: {
      page: 1,
      pageSize: 50,
      total: 0 as any,
    }
    //editRoleTitle: '',
  });


  onMounted(async () => {
    await handleQuery();
  });

  // 查询操作
  const handleQuery = async () => {
    state.loading = true;
    //let params = Object.assign(state.queryParams, state.tableParams);
    const res = await client.ApiAcmeInfoGetPage({});
    state.data = res?.items ?? [];
    state.tableParams.total = res?.total;
    state.loading = false;
  };

  // 打开新增页面
  const openAddAccount = () => {
    editAccountRef.value?.openDialog();
  };

  // 打开编辑页面
  const openEditAccount = async (row: any) => {
    editAccountRef.value?.openDialog(row);
  };

  // 删除
  const delAccount = (row: any) => {
    ElMessageBox.confirm(`确定删记录：【${row.email}】?`, '提示', {
      confirmButtonText: '确定',
      cancelButtonText: '取消',
      type: 'warning',
    })
      .then(async () => {
        await client.ApiAcmeInfoDelete(row.id);
        await handleQuery();
        ElMessage.success('删除成功');
      })
      .catch(() => { });
  };

  // 改变页面容量
  const handleSizeChange = async (val: number) => {
    state.tableParams.pageSize = val;
    await handleQuery();
  };

  // 改变页码序号
  const handleCurrentChange = async (val: number) => {
    state.tableParams.page = val;
    await handleQuery();
  };

</script>

<style lang="scss" scoped></style>
